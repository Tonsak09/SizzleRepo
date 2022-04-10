using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pullable : MonoBehaviour
{
    private enum PullableStates
    {
        passive,
        pulled,
        released
    };

    private PullableStates state;

    public QuadNode quadParent;


    // Start is called before the first frame update
    void Start()
    {
        state = PullableStates.passive;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PullableStates.passive:
                break;
            case PullableStates.pulled:
                break;
            case PullableStates.released:
                break;
        }
    }
}
