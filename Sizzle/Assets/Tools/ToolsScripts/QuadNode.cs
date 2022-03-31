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

        Add(collisions);
    }

    public void Add(Collider[] collisions)
    {
        // For each object get its verticies to see if box contains it 
        foreach (Collider col in collisions)
        {
            Vector3[] vertices = col.GetComponent<Mesh>().vertices;

            if (shapes.Count + 1 <= MAXOBJBEFOREDIVIDE)
            {
                shapes.Add(ProjectToPlane(vertices));
            }
            else
            {
                if (divisions == null)
                {
                    // Saves code and apart of group by default anyway 
                    shapes.Add(ProjectToPlane(vertices));

                    // Make divisions 

                    // Check smallest node that can be applied 
                }
                else
                {
                    // Check divisions for smallest that can be applied 

                    // If null or self add to this node 

                    // else add to the child 
                }
            }
        }
    }

    public bool BoxContains(List<Vector3> verticies)
    {
        foreach (Vector3 vertex in verticies)
        {
            // If the distance from the position is less
            // that scale in each axis it's within 

            // Todo -> Account for rotation 
        }

        return true;
    }

    private List<Vector3> ProjectToPlane(Vector3[] verticies)
    {
        List<Vector3> projections = new List<Vector3>();

        foreach (Vector3 vertex in verticies)
        {

        }
        // Add each point that lies within the plane 

        // If does not lie within the plane make sure to add edge intersections 

        return projections;
    }
}
