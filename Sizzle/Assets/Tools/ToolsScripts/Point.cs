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

    Vector3 neighborVecTest;
    Vector3 neighborPosVecTest;
    float tTest;

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
                /*
                // If at least one of the axis is the same as any of its neighbors allow move
                bool hasSimiliar = false;
                int sameVecCounter = 0;
                foreach (Point neighbor in neighbors)
                {

                    // If the vector between the new position and its opposite
                    // and the vector between the neighbors of this point can
                    // intersect/form a circle of a very small radius allow the change 

                    if (previousPos.y != this.transform.position.y)
                    {
                        break;
                    }
                    else if (neighbor.position.x == this.transform.position.x)
                    {
                        hasSimiliar = true;
                        break;
                    }
                    else if (neighbor.position.z == this.transform.position.z)
                    {
                        hasSimiliar = true;
                        break;
                    }
                    else
                    {
                        // Compare if vector between new pos and neighbor
                        // is the same as old pos and neighbor 
                        Vector3 newVec = this.transform.position - neighbor.position;
                        Vector3 oldVec = previousPos - neighbor.position;

                        float dot = Vector3.Dot(newVec.normalized, oldVec.normalized);

                        if ((1 - dot) < 0.01f)
                        {
                            print(dot);
                            if(sameVecCounter >= 2)
                            {
                                hasSimiliar = true;
                                break;
                            }
                            else
                            {
                                sameVecCounter++;
                            }
                        }
                    }
                }

                if(!hasSimiliar)
                {
                    // Otherwise revert to previousPos
                    this.transform.position = previousPos;
                }
                else
                {
                    UpdateSides();
                }
                */

                // For every tile attached too 
                foreach (MovementPlane plane in planes)
                {
                    // Get Vector between neighbors. 2 neighbors per plane 
                    List<Point> neighbors = GetNeighborsOnPlane(plane);
                    Vector3 neighborVec = neighbors[1].position - neighbors[0].position ;

                    // Get vector between this and its opposite 
                    Point opposite = GetOppositesOnPlane(plane);
                    Vector3 oppositeVec = this.position - opposite.position;

                    // Get t and s on any axis that is different 
                    float t = 0;
                    float s = 0;
                    /*
                    if(neighbors[0].position.x != opposite.position.x)
                    {
                        // Change in x axis 
                        t = (-neighbors[0].position + opposite.position).x / (-neighborVec + oppositeVec).x;
                        s = (-oppositeVec.x * neighbors[0].position.x + neighborVec.x * opposite.position.x) / (-neighborVec + oppositeVec).x; 
                    }
                    else if(neighbors[0].position.z != opposite.position.z)
                    {
                        // Change in z axis
                        t = (-neighbors[0].position + opposite.position).z / (-neighborVec + oppositeVec).z;
                        s = (-oppositeVec.z * neighbors[0].position.z + neighborVec.z * opposite.position.z) / (-neighborVec + oppositeVec).z;
                    }
                    else if(neighbors[0].position.y != opposite.position.y)
                    {
                        // Change in y axis 
                        t = (-neighbors[0].position + opposite.position).y / (-neighborVec + oppositeVec).y;
                        s = (-oppositeVec.y * neighbors[0].position.y + neighborVec.y * opposite.position.y) / (-neighborVec + oppositeVec).y;
                    }
                    */
                    t = (-neighbors[0].position + opposite.position).x / (-neighborVec + oppositeVec).x;
                    s = (-oppositeVec.x * neighbors[0].position.x + neighborVec.x * opposite.position.x) / (-neighborVec + oppositeVec).x; 

                    print(Vector3.Distance(-neighborVec * t, oppositeVec * s));

                    

                    neighborVecTest = neighborVec;
                    tTest = t;
                    neighborPosVecTest = neighbors[0].position;

                    if (Vector3.Distance(neighborVec * t, oppositeVec * s) > 0.1f)
                    {
                        this.transform.position = previousPos;
                    }
                    else
                    {
                        UpdateSides();
                    }
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

    public void UpdateSides()
    {
        foreach (Side side in sides)
        {
            side.UpdateOnPointMove();
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(-neighborVecTest * tTest + neighborPosVecTest, 0.1f);

    }
}
