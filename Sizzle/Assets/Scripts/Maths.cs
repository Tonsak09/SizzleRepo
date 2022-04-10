using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Maths
{
    public struct Shape
    {
        private Vector3 root;
        private List<Vector3> vertices;

        public Vector3 Root { get { return root; } }

        public List<Vector3> Verticies { get { return vertices; } }


        public Shape(Vector3 root, List<Vector3> vertices)
        {
            this.root = root;
            this.vertices = vertices;
        }
    }

    public static bool ShapeContainsPoint(Vector3 point, Shape shape)
    {
        // Check if there are enough verticies
        if(shape.Verticies.Count <= 2)
        {
            return false;
        }

        // Project to xz plane

        return true;
    }

    public static Vector3 GetIntersectionPoint(Vector3 originA, Vector3 offsetFromOriginA, Vector3 originB, Vector3 offsetFromOriginB)
    {

        // Now we have the closest side towards the origin 
        // Since we know for a fact that when this is called there is an intersection we can find
        // it here 

        // startA.x + t * offsetA.x = startB.x + u * offsetB.x
        // startA.z + t * offsetA.z = startB.z + u * offsetB.z

        // offsetA.x(t) - offsetB.x(u) = startB.x - startA.x
        // offsetA.z(t) - OffsetB.z(u) = startB.z - startA.z

        float[,] matrix2x2 = new float[2, 2];

        matrix2x2[0, 0] = offsetFromOriginA.x; // A
        matrix2x2[1, 0] = -offsetFromOriginB.x;// B
        matrix2x2[0, 1] = offsetFromOriginA.z; // C
        matrix2x2[1, 1] = -offsetFromOriginB.z;// D

        // Vector on other side of equation 
        Vector2 B = new Vector2(originB.x - originA.x, originB.z - originA.z);


        // Inverses the matrix 
        InverseMatrix2x2(matrix2x2);

        // Matrix x Vector 
        // (m[0, 0] * B[0]) + (m[0, 1] * b[1]) = t 
        // (m[1, 0] * B[0]) + (m[1, 1] * b[1]) = u

        float t = (matrix2x2[0, 0] * B[0]) + (matrix2x2[0, 1] * B[1]);
        float u = (matrix2x2[1, 0] * B[0]) + (matrix2x2[1, 1] * B[1]);

        // Makes sure that it actually leads to the correct point 
        if (((originA + offsetFromOriginA * t) - (originB + offsetFromOriginB * u)).magnitude <= 0.01f)
        {
            // Unless they intersect direction at (0, 0, 0) this should work 
            return Vector3.zero;
        }

        return originA + offsetFromOriginA * t;
    }

    /// <summary>
    /// Updates the inputed matrix to be its inverse 
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="det"></param>
    public static void InverseMatrix2x2(float[,] matrix)
    {
        float det = (matrix[0, 0] * matrix[1, 1]) - (matrix[1, 0] * matrix[0, 1]);

        // Swap A and D 
        float hold = matrix[0, 0];
        matrix[0, 0] = matrix[1, 1] / det;
        matrix[1, 1] = hold / det;

        // Make B and C negative 
        matrix[1, 0] = -matrix[1, 0] / det;
        matrix[0, 1] = -matrix[0, 1] / det;
    }


    /// <summary>
    /// If a point is within the plane formed by this quad.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool PlaneContainsPoint(Transform[] vertexPoints, Vector3 planeNormal, Vector3 point)
    {

        // See if within bounds of points 

        // At this stage every point has been projected onto the plane 

        // Solve by splitting the plane into two triangles and finding if the point is within them 
        Vector3[] triangleA = new Vector3[]
        {
            Vector3.ProjectOnPlane(vertexPoints[0].position, planeNormal),
            Vector3.ProjectOnPlane(vertexPoints[1].position, planeNormal),
            Vector3.ProjectOnPlane(vertexPoints[2].position, planeNormal)
        };


        Vector3 AB = triangleA[0] - triangleA[1];
        Vector3 AM = triangleA[0] - point;
        Vector3 BC = triangleA[1] - triangleA[2];
        Vector3 BM = triangleA[1] - point;

        float dotABAM = Vector3.Dot(AB, AM);
        float dotABAB = Vector3.Dot(AB, AB);
        float dotBCBM = Vector3.Dot(BC, BM);
        float dotBCBC = Vector3.Dot(BC, BC);

        return 0 <= dotABAM && dotABAM <= dotABAB && 0 <= dotBCBM && dotBCBM <= dotBCBC;
    }
}
