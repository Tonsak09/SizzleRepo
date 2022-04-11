using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConvexHull
{
    public static Stack<Vector3> GenerateHull(List<Vector3> points)
    {
        return GenerateHull(points.ToArray());
    }

    // Prints convex hull of a set of n points.
    public static Stack<Vector3> GenerateHull(Vector3[] points)
    {
        Stack<Vector3> hullStack = new Stack<Vector3>();

        List<Vector3> sortedPoints = SortPoints(points);
        Vector3 bottomost = GetBottomPoint(points);

        // Assumes that there is no duplicate 
        // When used with the quadNode duplicates are deleted but if used outside that be weary!
        sortedPoints.Remove(bottomost);
        hullStack.Push(bottomost);
        hullStack.Push(sortedPoints[0]);

        MonoBehaviour.print(sortedPoints[0]);

        for (int i = 1; i < sortedPoints.Count - 1; i++)
        {
            //Vector2 lineA = new Vector2((sortedPoints[i + 1] - sortedPoints[i]).x, (sortedPoints[i + 1] - sortedPoints[i]).z);
            //Vector2 lineB = new Vector2((sortedPoints[i + 2] - sortedPoints[i + 1]).x, (sortedPoints[i + 2] - sortedPoints[i + 1]).z);

            //MonoBehaviour.print(Maths.DetArea(new Vector2(sortedPoints[i - 1].x, sortedPoints[i - 1].z), new Vector2(sortedPoints[i].x, sortedPoints[i].z), new Vector2(sortedPoints[i + 1].x, sortedPoints[i + 1].z)));
            // Anticlockwise turn 
            if (Maths.DetArea(new Vector2(sortedPoints[i - 1].x, sortedPoints[i - 1].z), new Vector2(sortedPoints[i].x, sortedPoints[i].z), new Vector2(sortedPoints[i + 1].x, sortedPoints[i + 1].z)) < 0)
            {
                hullStack.Push(sortedPoints[i]);
            }
            else // Clowise turn 
            {
                hullStack.Pop();
            }

        }

        // Check with [0] and [count - 1]
        if (Maths.DetArea(new Vector2(sortedPoints[sortedPoints.Count - 2].x, sortedPoints[sortedPoints.Count - 2].z), new Vector2(sortedPoints[sortedPoints.Count - 1].x, sortedPoints[sortedPoints.Count - 1].z), new Vector2(sortedPoints[0].x, sortedPoints[0].z)) > 0)
        {
            hullStack.Push(sortedPoints[sortedPoints.Count - 1]);
        }
        else // Clowise turn 
        {
            //hullStack.Pop();
        }

        return hullStack;
    }

    // Gets value with lowest z value 
    private static Vector3 GetBottomPoint(Vector3[] points)
    {
        // Linear search due to unsorted and only need to find once 

        Vector3 lowest = points[0];
        for (int i = 0; i < points.Length; i++)
        {
            if(points[i].z < lowest.z)
            {
                lowest = points[i];
            }
        }

        return lowest;
    }

    /// <summary>
    /// Sorts the list by its x-axis 
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private static List<Vector3> SortPoints(Vector3[] points)
    {

        int n = points.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (points[j].x > points[j + 1].x)
                {
                    // swap temp and arr[i]
                    Vector3 temp = points[j];
                    points[j] = points[j + 1];
                    points[j + 1] = temp;
                }

        return new List<Vector3>(points);
    }
}
