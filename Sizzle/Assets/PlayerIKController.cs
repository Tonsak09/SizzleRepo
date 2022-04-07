using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKController : MonoBehaviour
{
    public Player player;
    public Transform root;

    // [0] - Front
    // [1] - Back, real position 

    public Transform[] head;
    public Vector3 headFrontOffset;
    public Vector3 headBacktOffset;

    public Transform[] upperBody;
    public Vector3 upperBodyFrontOffset;
    public Vector3 upperBodyBackOffset;

    public Transform[] lowerBody;
    public Vector3 lowerBodyFrontOffset;
    public Vector3 lowerBodyBackOffset;

    public Transform[] midTail;
    public Vector3 midTailFrontOffset;
    public Vector3 midTailBackOffset;

    public Transform[] endTail;
    public Vector3 endTailFrontOffset;
    public Vector3 endTailBackOffset;

    private Transform[][] bodyChain;

    private Dictionary<Transform[], float> PairDistance;
    

    private void Awake()
    {

        bodyChain = new Transform[][]
        {
            head,
            upperBody,
            lowerBody,
            midTail,
            endTail
        };

        PairDistance = new Dictionary<Transform[], float>();
        for (int i = 0; i < bodyChain.Length; i++)
        {
            // Stores each pairs initial distance form one another 
            PairDistance.Add(bodyChain[i], (bodyChain[i][0].position - bodyChain[i][1].position).magnitude);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Need to accomodate planes with different normals
        head[1].position = root.transform.position + player.curentParentTile.plane.normal * headBacktOffset.magnitude;
        print(headFrontOffset);
        foreach (Transform[] part in bodyChain)
        {
            UpdateChainPart(part);
            print(part);
        }

    }

    private void UpdateChainPart(Transform[] part)
    {
        part[1].LookAt(part[0].position, player.curentParentTile.plane.normal);

        float change = Vector3.Distance(part[0].position, part[1].position) - PairDistance[part];

        // If distance is brocken then adjust point to make distance normal 
        if (change > 0.01f)
        {
            Vector3 vec = (part[1].position - part[0].position).normalized;
            part[1].position -= vec * change;
        }
    }
}
