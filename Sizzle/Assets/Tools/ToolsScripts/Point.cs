using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Point : MonoBehaviour
{

    public Vector3 position { get { return this.transform.position; } set { this.transform.position = value; } }

    public List<Point> opposites;
    public List<Point> neighbors;
    public List<Side> sides;
    public List<MovementPlane> planes;

    private Vector3 previousPos;

    public enum ProtectPlaneForms
    {
        solo, // Can only be moved in axis that does not destroy plane, no influence on other points 
        oppositeReverse, // Opposite points are moved in the opposite vector as this point 
        oppositeCopy, // Opposite points are moved in the same vector as this point 
    };

    public ProtectPlaneForms MoveForm;

    // Start is called before the first frame update
    void Start()
    {
        previousPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeState();

        if(this.transform.position != previousPos)
        {
            UpdatePositions();
        }

        previousPos = this.transform.position;
    }

    private void UpdatePositions()
    {
        switch (MoveForm)
        {
            case ProtectPlaneForms.solo:


                bool coplaner = true; 
                foreach (MovementPlane plane in planes)
                {
                    List<Point> neighbors = GetNeighborsOnPlane(plane);
                    Point opposite = GetOppositesOnPlane(plane);

                    if(!IsCoplaner(neighbors[0].position, neighbors[1].position, opposite.position, this.position))
                    {
                        this.transform.position = previousPos;

                        coplaner = false;
                        break;
                    }
                }
                if(coplaner)
                {
                    UpdateSides();
                }
                
                break;
            case ProtectPlaneForms.oppositeReverse:

                // Get Vector difference between new and old position
                Vector3 difference = previousPos - this.transform.position;

                foreach (Point point in opposites)
                {
                    // Apply it in opposite direction to point
                    point.position += difference;
                }
                break;
            case ProtectPlaneForms.oppositeCopy:
                // Get Vector difference between new and old position
                difference =  this.transform.position - previousPos;

                foreach (Point point in opposites)
                {
                    // Apply it in opposite direction to point
                    point.position += difference;
                }
                break;
            default:
                break;
        }
        
    }

    private bool IsCoplaner(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        // I have no idea how this works I stole it from 
        // Geeksforgeeks and I feel bad. NEED TO LEARN 

        float a1 = p2.x - p1.x;
        float b1 = p2.y - p1.y;
        float c1 = p2.z - p1.z;
        float a2 = p3.x - p1.x;
        float b2 = p3.y - p1.y;
        float c2 = p3.z - p1.z;
        float a = b1 * c2 - b2 * c1;
        float b = a2 * c1 - a1 * c2;
        float c = a1 * b2 - b1 * a2;
        float d = (-a * p1.x - b * p1.y - c * p1.z);

        // equation of plane is: a*x + b*y + c*z = 0 #

        // checking if the 4th point satisfies
        // the above equation
        if (a * p4.x + b * p4.y + c * p4.z + d == 0)
            return true;
        else
            return false;
    }

    public void UpdateSides(Side ignore = null)
    {
        foreach (Side side in sides)
        {
            if(side != ignore)
            {
                side.UpdateOnPointMove();
            }
        }
    }

    private void ChangeState()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            MoveForm = ProtectPlaneForms.solo;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MoveForm = ProtectPlaneForms.oppositeReverse;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MoveForm = ProtectPlaneForms.oppositeCopy;
        }

    }

    public void OverridePosition(Vector3 newPos)
    {
        this.transform.position = newPos;
        previousPos = this.transform.position;
    }

    public List<Point> GetNeighborsOnPlane(MovementPlane plane)
    {
        List<Point> points = new List<Point>();

        foreach (Point neighbor in neighbors)
        {
            if(neighbor.planes.Contains(plane))
            {
                points.Add(neighbor);
            }
        }

        return points;
    }

    public Point GetOppositesOnPlane(MovementPlane plane)
    {
        List<Point> points = new List<Point>();

        foreach (Point opposite in opposites)
        {
            if (opposite.planes.Contains(plane))
            {
                return opposite;
            }
        }

        return null;
    }
}
