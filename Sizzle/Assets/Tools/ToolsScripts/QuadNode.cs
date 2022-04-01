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
    private List<List<Vector3>> shapes;
    private QuadNode[] divisions;

    public bool generate;

    // Just so doesn't appear in the editor but still accesible by other scripts 
    public List<List<Vector3>> Shapes { get { return shapes; } set { shapes = value; } }
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
        if (generate)
        {
            GenerateNodes();

            generate = false;
        }
    }

    public void GenerateNodes()
    {
        // Get all objects within the box 
        Collider[] collisions = Physics.OverlapBox(this.transform.position, this.transform.localScale, this.transform.rotation, mask);
        foreach (Collider col in collisions)
        {
            // Get verticies that actually exist 
            Add(ProjectToPlane(col.GetComponent<Mesh>().vertices));
        }
    }

    public void Add(List<Vector3> vertices)
    {
        // For each object get its verticies to see if box contains it 
        
        if (shapes.Count + 1 <= MAXOBJBEFOREDIVIDE)
        {
            shapes.Add(vertices);
        }
        else
        {
            if (divisions == null)
            {
                // Saves code and apart of group by default anyway 
                shapes.Add(vertices);

                // Make divisions 
                Divide();
            }
            else
            {
                // Check divisions for smallest that can be applied 

                // If null or self add to this node 

                // else add to the child 
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
                        y * (this.transform.localScale.y / 2) +(this.transform.localScale.y / 4),
                        0 // No change from root z
                    ), 
                    Quaternion.identity
                );

                // Does not need to affect z axis**** 
                nodeTemp.transform.localScale /= 2;

                divisions[x % 1 + y / 2] = nodeTemp.GetComponent<QuadNode>();
            }
        }

        foreach (List<Vector3> shape in shapes)
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
    public QuadNode SmallestNode(List<Vector3> vertices)
    {
        if (PlaneContainsShape(vertices))
        {
            if (divisions != null)
            {
                // Iterate through each child to see if can fit into any of these
                foreach (QuadNode child in divisions)
                {
                    if (child.PlaneContainsShape(vertices))
                    {
                        return SmallestNode(vertices);
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


    /// <summary>
    /// If a point is within the plane formed by this quad.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool PlaneContainsPoint(Vector3 point)
    {
        // See if within bounds of points 

        return true;
    }

    /// <summary>
    /// Checks if a series of vertexes fit into a 
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public bool PlaneContainsShape(List<Vector3> shape)
    {

        return true;
    }

    /// <summary>
    /// Simplifies the vertices to a represent a shape along the up most
    /// plane. If vertices go out of up most plane they are cut 
    /// </summary>
    /// <param name="verticies"></param>
    /// <returns></returns>
    private List<Vector3> ProjectToPlane(Vector3[] verticies)
    {
        List<Vector3> projections = new List<Vector3>();

        foreach (Vector3 vertex in verticies)
        {
            if(PlaneContainsPoint(vertex))
            {
                projections.Add(Vector3.ProjectOnPlane(vertex, plane.normal));

            }
        }
        // Add each point that lies within the plane 

        // If does not lie within the plane make sure to add edge intersections 

        return projections;
    }

    
}
