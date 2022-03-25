using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Point : MonoBehaviour
{

    public Vector3 position { get { return this.transform.position; } }

    public List<Point> opposites;
    public List<Point> neighbors;

    private Vector3 previousPos;

    public enum ProtectPlaneForms
    {
        solo, // Can only be moved in axis that does not destroy plane, no influence on other points 
        oppositeReverse, // Opposite points are moved in the opposite vector as this point 
        neighborReverse, // Neighbor points are moved in the opposite vector as this point 
        oppositeCopy, // Opposite points are moved in the same vector as this point 
        neighborCopy  // Neighbor points are moved in the same vector as this point 
    };

    public ProtectPlaneForms MoveForm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePositions()

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
                    if(neighbor.position.x == this.transform.position.x)
                    {
                        hasSimiliar = true;
                        break;
                    }
                    else if(neighbor.position.z == this.transform.position.z)
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

                        if(newVec.normalized == oldVec.normalized)
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

                break;
            case ProtectPlaneForms.oppositeReverse:
                break;
            case ProtectPlaneForms.neighborReverse:
                break;
            case ProtectPlaneForms.oppositeCopy:
                break;
            case ProtectPlaneForms.neighborCopy:
                break;
            default:
                break;
        }
    }

    private void ChangeState()
    {

    }
}
