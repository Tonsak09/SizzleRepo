using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class QuadNode : MonoBehaviour
{

    // The bottom four point that show the actualy plane 
    public Transform[] points;
    public LayerMask mask;

    private const int MAXOBJBEFOREDIVIDE = 3;

    // Stores list of barycentric coords that player cannot go within 
    private List<Shape> shapes;
    public QuadNode[] divisions;

    public GameObject quadNodePrefab;

    public bool generate;

    // Just so doesn't appear in the editor but still accesible by other scripts 
    public List<Shape> Shapes { get { return shapes; } set { shapes = value; } }
    private Plane plane 
    { 
        get 
        {
            return new Plane(points[0].position, points[1].position, points[2].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //generate = false;
        if (generate)
        {
            generate = false;

            shapes = new List<Shape>();

            List<QuadNode> children = GetDivisions();

            foreach (QuadNode child in children)
            {
                DestroyImmediate(child.gameObject);
            }

            this.divisions = null;

            GenerateNodes();
            print("Generated");
        }
    }

    public void GenerateNodes()
    {
        // Get all objects within the box 
        Collider[] collisions = Physics.OverlapBox(this.transform.position, this.transform.localScale, this.transform.rotation, mask);
        // Also need the root position

        foreach (Collider col in collisions)
        {
            // Get verticies that actually exist in area 
            Vector3[] planePositions = ProjectToPlane(col.transform.position, col.GetComponent<MeshFilter>().sharedMesh.vertices).ToArray();
            Add(new Shape( col.transform.position, planePositions));
        }
    }

    public void Add(Shape shape)
    {
        // For each object get its verticies to see if box contains it 
        
        if (shapes.Count + 1 < MAXOBJBEFOREDIVIDE)
        {
            shapes.Add(shape);
            
        }
        else
        {
            if (divisions == null || divisions.Length == 0)
            {
                // Saves code and apart of group by default anyway 
                shapes.Add(shape);

                // Make divisions 
                Divide();
            }
            else
            {
                // Check divisions for smallest that can be applied 
                QuadNode smallestNode = SmallestNode(shape);

                // If null or self add to this node 
                if(smallestNode == null || smallestNode == this)
                {
                    shapes.Add(shape);
                }
                else
                {
                    // else add to the child 
                    smallestNode.shapes.Add(shape);
                }

            }
        }
        
    }

    private void Divide()
    {
        divisions = new QuadNode[4];

        // Works because it comes from a unit cube 
        Vector3 root = this.transform.position - (this.transform.localScale / 2);

        // Split up and allign position with vector from pos[0]
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {

                // Centers new node in middle of quadrant but same z axis
                GameObject nodeTemp = Instantiate(this.gameObject, root +
                    new Vector3
                    (
                        x * (this.transform.localScale.x / 2) + (this.transform.localScale.x / 4), 
                        0,//y * (this.transform.localScale.y / 2) +(this.transform.localScale.y / 4),
                        y * (this.transform.localScale.z / 2) + (this.transform.localScale.z / 4)
                    ), 
                    Quaternion.identity
                );

                nodeTemp.GetComponentInParent<QuadNode>().divisions = null;

                // Does not need to affect z axis**** 
                nodeTemp.transform.localScale /= 2;
                nodeTemp.transform.position += (nodeTemp.transform.localScale.y / 2) * plane.normal;

                divisions[x + (y * 2)] = nodeTemp.GetComponent<QuadNode>();
            }
        }

        foreach (Shape shape in shapes)
        {
            if (PlaneContainsShape(shape))
            {
                QuadNode smallest = SmallestNode(shape);

                if (smallest != null || smallest != this)
                {
                    smallest.Add(shape);
                }
            }
            
            // Already apart of group so need to re-add 
        }
    }

    /// <summary>
    /// Gets the smallest node that can conatin the given vertices 
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public QuadNode SmallestNode(Shape shape)
    {
        if (PlaneContainsShape(shape))
        {
            if (divisions != null)
            {
                // Iterate through each child to see if can fit into any of these
                foreach (QuadNode child in divisions)
                {
                    if (child.PlaneContainsShape(shape))
                    {
                        return SmallestNode(shape);
                    }
                }

                // No child can fit 
                return this;
            }
            else
            {
                return this;
            }
        }

        return null;
    }

    public List<QuadNode> GetDivisions()
    {
        List<QuadNode> nodes = new List<QuadNode>();
        if(divisions != null)
        {
            foreach (QuadNode quad in divisions)
            {
                if(quad != null)
                {
                    nodes.Add(quad);
                    if (quad.divisions != null)
                    {
                        nodes.AddRange(quad.GetDivisions());
                    }
                }
            }
        }

        return nodes;
    }


    /// <summary>
    /// If a point is within the plane formed by this quad.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool PlaneContainsPoint(Vector3 point)
    {
        // See if within bounds of points 

        // At this stage every point has been projected onto the plane 

        // Solve by splitting the plane into two triangles and finding if the point is within them 
        Vector3[] triangleA = new Vector3[]
        {
            Vector3.ProjectOnPlane(points[0].position, plane.normal),
            Vector3.ProjectOnPlane(points[1].position, plane.normal),
            Vector3.ProjectOnPlane(points[2].position, plane.normal)
        };

        Vector3[] triangleB = new Vector3[]
        {
            Vector3.ProjectOnPlane(points[3].position, plane.normal),
            Vector3.ProjectOnPlane(points[1].position, plane.normal),
            Vector3.ProjectOnPlane(points[2].position, plane.normal)
        };

        print(TriangleContainsPoint(point, triangleA) + " : " + TriangleContainsPoint(point, triangleB));

        return TriangleContainsPoint(point, triangleA) || TriangleContainsPoint(point, triangleB);
    }

    /// <summary>
    /// Checks if a series of vertexes fit into the current plane 
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public bool PlaneContainsShape(Shape shape)
    {
        bool contains = true;
        // stack overflow 

        foreach (Vector3 point in shape.Verticies)
        {
            if (!PlaneContainsPoint(point))
            {
                contains = false;
                break;
            }
        }

        return contains;
    }


    /// <summary>
    /// Checks to see if a point in 3d space exists within or out of 3 vertices 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="vertices"></param>
    /// <returns></returns>
    private bool TriangleContainsPoint(Vector3 point, Vector3[] vertices)
    {
        /*
        a = ((y2 - y3) * (x - x3) + 
        (x3 - x2) * (y - y3)) 
        / 
        ((y2 - y3) * (x1 - x3) + 
        (x3 - x2) * (y1 - y3))

        b = 
        ((y3 - y1) * (x - x3) + 
        (x1 - x3) * (y - y3)) 
        / 
        ((y2 - y3) * (x1 - x3) + 
        (x3 - x2) * (y1 - y3))

        c = 1 - a - b
        */

        float denom = (vertices[1].y - vertices[2].y) * (vertices[0].x - vertices[2].x) +
                (vertices[2].x - vertices[1].x) * (vertices[0].z - vertices[2].z);


        float a = (
                ((vertices[1].z - vertices[2].z) * (point.x - vertices[2].x) +
                (vertices[2].x - vertices[1].x) * (point.z - vertices[2].z)) 
                /
                (denom)
            );

        float b = (
                ((vertices[2].z - vertices[0].z) * (point.x - vertices[2].x) +
                (vertices[0].x - vertices[2].x) * (point.y - vertices[2].z))
                /
                (denom)
            );

        float c = 1 - a - b;

        bool caseA = (0 <= a) && (a <= 1);
        bool caseB = (0 <= b) && (b <= 1);
        bool caseC = (0 <= c) && (c <= 1);

        return caseA && caseB && caseC;
    }

    
    /// <summary>
    /// Simplifies the vertices to a represent a shape along the up most
    /// plane. If vertices go out of up most plane they are cut 
    /// </summary>
    /// <param name="verticies"></param>
    /// <returns></returns>
    private List<Vector3> ProjectToPlane(Vector3 root, Vector3[] verticies)
    {
        List<Vector3> projections = new List<Vector3>();

        foreach (Vector3 vertex in verticies)
        {
            // Add each point that lies within the plane 
            print(Vector3.ProjectOnPlane(root + vertex, plane.normal));
            if (PlaneContainsPoint(Vector3.ProjectOnPlane(root + vertex, plane.normal)))
            {
                projections.Add(Vector3.ProjectOnPlane(root + vertex, plane.normal));
            }
        }

        print("Length of projects: " + projections.Count);
        // If does not lie within the plane make sure to add edge intersections TODO****

        return projections;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(shapes != null)
        {
            foreach (Shape shape in shapes)
            {
                foreach (Vector3 point in shape.Verticies)
                {
                    Gizmos.DrawWireSphere(point, 0.1f);
                }
            }
        }
        else
        {
            print("Shapes is null");
        }
        
    }

    public struct Shape
    {
        private Vector3 root;
        private Vector3[] vertices;

        public Vector3 Root { get { return root; } }
        public Vector3[] Verticies 
        { 
            get 
            {
                Vector3[] UsefulVerticies = new Vector3[vertices.Length];

                for (int i = 0; i < UsefulVerticies.Length; i++)
                {
                    UsefulVerticies[i] = root + vertices[i];
                }

                return UsefulVerticies;
            } 
        }

        public Shape(Vector3 root, Vector3[] vertices)
        {
            this.root = root;
            this.vertices = vertices;
        }
    }
}
