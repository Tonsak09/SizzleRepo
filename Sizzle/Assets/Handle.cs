using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    public Pullable pullable;
    public Vector3 normal;
    public Plane Directionplane { get { return new Plane(this.transform.position, pullable.transform.position, pullable.transform.position + normal); } }

    public Transform block;

    public ParticleSystem sparksFX;
}
