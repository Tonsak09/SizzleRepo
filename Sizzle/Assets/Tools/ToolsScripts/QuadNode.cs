using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int shapesCount = 0;

    public Vector3 xAxis { get { return (points[1].position - points[2].position).normalized; } }
    public Vector3 yAxis { get { return (points[0].position - points[1].position).normalized; } }


    public bool generate;

    // Just so doesn't appear in the editor but still accesible by other scripts 
    public List<Shape> Shapes { get { return shapes; } set { shapes = value; } }
    public Plane plane 
    { 
        get 
        {
            return new Plane(points[0].position, points[1].position, points[2].position);
        }
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(shapes != null)
        {
            shapesCount = shapes.Count;
        }

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
        }
    }

    public void GenerateNodes()
    {
        // Get all objects within the box 
        Collider[] collisions = Physics.OverlapBox(this.transform.position, this.transform.localScale / 2, this.transform.rotation, mask);

        // Also need the root position
        Vector3[] planePositions = null;


        foreach (Collider col in collisions)
        {
            // Get verticies that actually exist in area 
            planePositions = ProjectToPlane(col.transform.position, col.GetComponent<MeshFilter>().sharedMesh.vertices).ToArray();
            Add(new Shape( col.transform.position, planePositions));
        }
    }

    public void Add(Shape shape)
    {
        // For each object get its verticies to see if box contains it 
        
        if(PlaneContainsShape(shape))
        {
            
            if (divisions == null || divisions.Length == 0)
            {
                shapes.Add(shape);

                if (shapes.Count >= MAXOBJBEFOREDIVIDE)
                {
                    Divide();
                }
            }
            else
            {
                // Check divisions for smallest that can be applied 
                QuadNode smallestNode = SmallestNode(shape);

                // If null or self add to this node 
                if (smallestNode != null && smallestNode != this)
                {

                    smallestNode.Add(shape);
                    
                    shapes.Remove(shape);
                }
                else
                {
                    shapes.Add(shape);
                }
            }
        }
        else
        {
            shapes.Add(shape);
        }
    }

    public void Divide()
    {
        divisions = new QuadNode[4];

        // Works because it comes from a unit cube 
        Vector3 root = points[2].position; //this.transform.position - (this.transform.localScale / 2);

        

        // Split up and allign position with vector from pos[0]
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {

                // Centers new node in middle of quadrant but same z axis
                GameObject nodeTemp = Instantiate(quadNodePrefab, root,
                    this.transform.rotation
                ); ;


                nodeTemp.GetComponent<QuadNode>().generate = false;
                nodeTemp.GetComponent<QuadNode>().divisions = null;
                nodeTemp.GetComponent<QuadNode>().shapes = new List<Shape>();

                // Does not need to affect z axis**** 
                nodeTemp.transform.localScale /= 2;

                //this.transform.position += nodeTemp.transform.localScale.x * x * xAxis;
                //this.transform.position += nodeTemp.transform.localScale.y * y * yAxis;

                nodeTemp.transform.position += (nodeTemp.transform.localScale.y) * plane.normal;
                nodeTemp.transform.localScale += Vector3.up * nodeTemp.transform.localScale.y;

                nodeTemp.transform.position += yAxis * nodeTemp.transform.localScale.z / 2 + (y * yAxis * nodeTemp.transform.localScale.z);
                nodeTemp.transform.position += xAxis * nodeTemp.transform.localScale.x / 2 + (x * xAxis * nodeTemp.transform.localScale.x);

                divisions[x + (y * 2)] = nodeTemp.GetComponent<QuadNode>();
            }
        }
        for (int i = 0; i < shapes.Count; i++)
        {
            if (PlaneContainsShape(shapes[i]))
            {
                QuadNode smallest = SmallestNode(shapes[i]);

                if (smallest != null && smallest != this)
                {
                    smallest.Add(shapes[i]);
                    this.shapes.RemoveAt(i);
                    i--;
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
                QuadNode quad = null;
                // Iterate through each child to see if can fit into any of these
                foreach (QuadNode child in divisions)
                {

                    if (child.PlaneContainsShape(shape))
                    {
                        quad = child.SmallestNode(shape);
                    }
                }

                if(quad == null)
                {
                    // No child can fit 
                    return this;
                }
                else
                {
                    return quad;
                }
            }
            else
            {
                // Leaf in tree
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
        else
        {
            nodes.Add(this);
        }

        return nodes;
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
            Vector3 projection = Vector3.ProjectOnPlane(root + vertex, plane.normal);

            if (!projections.Contains(projection))
            {
                if (PlaneContainsPoint(projection))
                {
                    projections.Add(projection);
                }
            }
            /*
            // Add each point that lies within the plane 
            if (PlaneContainsPoint(projection))
            {
                //projections.Add(Vector3.ProjectOnPlane(root + vertex, plane.normal));
                //print("Does contain " + projections[projections.Count - 1]);
            }
            else
            {
                //print("Does not contain point " + Vector3.ProjectOnPlane(root + vertex, plane.normal));
            }
            */
        }

        //print("Length of projects: " + projections.Count + ". Comes from " + this.transform.position);
        // If does not lie within the plane make sure to add edge intersections TODO****

        return projections;
    }

    /// <summary>
    /// Checks if a series of vertexes fit into the current plane 
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public bool PlaneContainsShape(Shape shape)
    {
        bool contains = true;


        // Go through each point and see if plane contains it 
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

    public Vector3 MoveAlongTile(Vector3 point, Player player)
    {
        // Sets up Movealongtile with correct values but also means that 
        // working with the player class is less confusing 
        return MoveAlongTile(Vector3.ProjectOnPlane(point, plane.normal), Vector3.ProjectOnPlane(player.transform.position, plane.normal), player);
    }

    /// <summary>
    /// Returns the next possible positions that player can be 
    /// </summary>
    /// <param name="point">MUST be projected onto the plane</param>
    /// <param name="player"></param>
    /// <returns></returns>
    private Vector3 MoveAlongTile(Vector3 point, Vector3 origin, Player player)
    {
        List<QuadNode> quadsPointExistsIn = GetQuadsThatPointExists(point);
        Vector3 nextPoint = Vector3.negativeInfinity; // Default value 

        
        if(!PlaneContainsPoint(point))
        { 
            // Check if neighbor tile in that direction 

            // If not reflect player off edge and repeat function with new point and origin from edge 
        }

        foreach (QuadNode quad in quadsPointExistsIn)
        {
            foreach (Shape shape in quad.shapes) 
            {
                if(ShapeContainsPoint(point, shape)) // Cannot currently work with overlaping obstacles 
                {
                    // Check where the collision takes places and reflect of that 

                    // Find side where intersection happens 
                    Vector3 travelLine = point - origin;

                    //Vector3 intersection = GetIntersectionPoint(travelLine, shape);
                    //float newMag = travelLine.magnitude - (intersection - origin).magnitude;

                    // Whip camera around to new direction 

                    break;
                }
            }

            if(nextPoint != Vector3.negativeInfinity)
            {
                break;
            }

        }

        // No intersections or collisions 
        if(nextPoint == Vector3.negativeInfinity)
        {
            return point;
        }

        return nextPoint;

    }

    public List<QuadNode> GetQuadsThatPointExists(Vector3 point)
    {
        List<QuadNode> nodes = new List<QuadNode>();
        if(PlaneContainsPoint(point))
        {
            nodes.Add(this);
            if (divisions != null)
            {
                foreach (QuadNode node in divisions)
                {
                    List<QuadNode> childNodes = node.GetQuadsThatPointExists(point);
                    if(childNodes != null)
                    {
                        nodes.AddRange(childNodes);
                        break;
                    }
                }
            }

            return nodes;
        }
        else
        {
            return null;
        }
    }

    private bool ShapeContainsPoint(Vector3 point, Shape shape)
    {
        // Check if there are enough verticies

        // Project to xz plane

        return true;
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
    }

    public struct Shape
    {
        private Vector3 root;
        private Vector3[] vertices;

        public Vector3 Root { get { return root; } }
        public Vector3[] Verticies { get { return vertices; } }
        public Vector3[] VerticiesWithRoot 
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
