using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConvexHull
{
    

    // Prints convex hull of a set of n points.
    public static List<Vector3> GenerateHull(Vector3[] points)
    {
        Stack<Vector3> hullStack = new Stack<Vector3>();
        List<Vector3> hull = new List<Vector3>();

        List<Vector3> sortedPoints = SortPoints(points);
        Vector3 bottomost = GetBottomPoint(points);

        return hull;
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
        List<Vector3> sortedList = new List<Vector3>();

        return sortedList;
    }
}
