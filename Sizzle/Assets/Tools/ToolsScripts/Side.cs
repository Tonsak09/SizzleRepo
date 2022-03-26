using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Side : MonoBehaviour
{

    public List<Point> points;

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

        if(previousScale != this.transform.localScale)
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
            Vector3 difference =  this.transform.position - previousPos;
            point.OverridePosition(point.position + difference);
        }
    }

    private void UpdatePositionsFromScale()
    {

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
