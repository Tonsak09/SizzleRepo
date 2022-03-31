using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Side : MonoBehaviour
{

    public List<Point> points;
    public List<Side> sides;

    private Vector3 previousPos;
    private Vector3 previousScale;

    private void Start()
    {
        previousPos = this.transform.position;
        previousScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (previousPos != this.transform.position)
        {
            UpdatePositionsFromPosition();
        }

        if (previousScale != this.transform.localScale)
        {
            UpdatePositionsFromScale();
        }

        previousPos = this.transform.position;
        previousScale = this.transform.localScale;
    }

    private void UpdatePositionsFromPosition()
    {
        foreach (Point point in points)
        {
            Vector3 difference = this.transform.position - previousPos;
            point.OverridePosition(point.position + difference);

            point.UpdateSides(this);
        }
    }

    private void UpdatePositionsFromScale()
    {
        // Average scale change
        float averageChange = (this.transform.localScale.x + this.transform.localScale.y + this.transform.localScale.z) / 3;

        ChangePointPositions(this, averageChange);

        foreach(Side side in sides)
        {
            ChangePointPositions(side, averageChange);
        }
        // After everything moved readjust 
        foreach (Side side in sides)
        {
            foreach (Point point in side.points)
            {
                point.UpdateSides(side);
            }
        }
    }

    private void ChangePointPositions(Side side, float change)
    {
        Vector3 center = (side.points[0].position + side.points[1].position) / 2;

        side.points[0].position += (side.points[0].position - center).normalized * change;
        side.points[1].position += (side.points[1].position - center).normalized * change;

        side.transform.localScale = Vector3.one;
        side.previousScale = Vector3.one;
    }

    public void UpdateOnPointMove()
    {
        Vector3 sum = Vector3.zero;
        foreach (Point point in points)
        {
            sum += point.position;
        }

        this.transform.position = sum / points.Count;

        previousPos = this.transform.position;
        previousScale = this.transform.localScale;
    }
}