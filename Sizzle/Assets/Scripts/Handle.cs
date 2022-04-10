using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    public Pullable pullable;
    private Vector3 normal { get { return pullable.quadParent.plane.normal; } }
    public Plane Directionplane { get { return new Plane(this.transform.position, pullable.transform.position, pullable.transform.position + normal); } }

    public Transform block;

    public ParticleSystem sparksFX;
}
