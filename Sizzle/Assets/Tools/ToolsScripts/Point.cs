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

                // If at least one of the axis is the same as any of its neighbors allow move
                bool hasSimiliar = false;
                foreach (Point neighbor in neighbors)
                {
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

                        float dot = Vector3.Dot(newVec, oldVec);

                        if ((1 - dot) < 0.1f)
                        {
                            hasSimiliar = true;
                            break;
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
}
