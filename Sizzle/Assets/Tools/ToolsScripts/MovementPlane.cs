using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MovementPlane : MonoBehaviour
{

    // Todo - Override points position if moving whole plane 

    [Header("Plane Information")]
    public List<Point> points;
    public List<Side> sides;
    public List<MovementPlane> connectedPlanes;

    public LayerMask mask;

    private Player player;

    public enum DrawModes
    {
        none, 
        wireFrame,
        collision
    };

    [Header("Collision/Path Generation")]
    public float boxLength;
    public float boxHeight;

    [Header("Rendering")]
    public DrawModes DrawMode;

    [Header("Generation")]
    public bool generate;

    // Update is called once per frame
    void Update()
    {

        switch (DrawMode)
        {
            case DrawModes.none:
                break;
            case DrawModes.wireFrame:
                DrawWireFrame();
                break;
            case DrawModes.collision:
                // Draw boxes at each point where collision is detected 
                break;
        }


        // yes i know that you can make buttons. If you would like
        // to add that into the inspector be my guest 
        if(generate)
        {
            Generate();

            generate = false;
        }

    }

    /// <summary>
    /// Creates a wireframe of the plane
    /// </summary>
    private void DrawWireFrame()
    {
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {
                if(i != j)
                {
                    Debug.DrawLine(points[i].position, points[j].position);
                }
            }
        }
    }

    /// <summary>
    /// Checks each piece of the plane to see if there is collision with the mask.
    /// At the end stores sections that the player cannot go into 
    /// </summary>
    private void Generate()
    {
        // Get negative most in all axis 

        // Move along one direction until reach past plane

        // Hold list of all Vector3 positions that detected collsions
    }
}
