using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public enum PlayerStates
    { 
        Idle,
        DefaultMove,
        Dash,
        HoldIdle,
        HoldMove,
    }

    public PlayerStates state;


    public QuadNode curentParentTile;

    public Transform cam;

    public float rotationSpeed;
    public float defaultSpeed;
    public float holdSpeed;

    public float timeToMaxSpeed;
    private float a;
    private Vector3 realSpeed; // set to getter and setter

    public Vector3 direction;

    public GameObject sparkFX;

    public Transform head;
    public Transform grabPoint;

    public LayerMask grabbale;
    private Plane grabbedPlane;
    private Transform pullable;
    private ParticleSystem blockSparks;

    private Transform[][] bodyChain;

    public Vector3 RealSpeed { get { return realSpeed; } set { realSpeed = value; } }

    // Start is called before the first frame update
    void Start()
    {
        bodyChain = this.GetComponent<PlayerIKController>().BodyChain;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Snap Player to plane
        this.transform.position = curentParentTile.transform.position;

        direction = Vector3.ProjectOnPlane(Vector3.right, curentParentTile.plane.normal).normalized;

        a = defaultSpeed / timeToMaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        // Always make sure that the head is within a certain range direction from the center and can't just completely turn around 
        //AdjustAngles();

        AdjustHead();

        switch (state)
        {
            case PlayerStates.Idle:
                // Play idle animation 

                Spark();

                if (Move(defaultSpeed))
                {
                    state = PlayerStates.DefaultMove;
                }

                if (Hold())
                {
                    state = PlayerStates.HoldIdle;
                }


                break;
            case PlayerStates.DefaultMove:

                Spark();

                if (!Move(defaultSpeed))
                {
                    state = PlayerStates.Idle;
                }

                if (Hold())
                {
                    state = PlayerStates.HoldMove;
                }

                break;
            case PlayerStates.Dash:

                // Start animation if not running 
                // When over return to move 

                state = PlayerStates.DefaultMove;

                break;
            case PlayerStates.HoldIdle:

                if (!Hold())
                {
                    state = PlayerStates.Idle;
                }

                if (HoldMove(holdSpeed, grabbedPlane))
                {
                    state = PlayerStates.HoldMove;
                }

                break;
            case PlayerStates.HoldMove:

                if (!blockSparks.isPlaying)
                {
                    blockSparks.Play();
                }


                if (!HoldMove(holdSpeed, grabbedPlane))
                {
                    state = PlayerStates.HoldIdle;
                    blockSparks.Stop();
                }

                if (!Hold())
                {
                    state = PlayerStates.DefaultMove;
                    blockSparks.Stop();
                }


                break;
            default:
                break;
        }
    }

    /*
    private const float HEADTURNTIME = 0.2f;
    private float turnSpeed = 0.1f;
    private float startTime;
    private Quaternion startRot;

    private Quaternion holdLookRot; // Used to see if rotation has been updated 
    */
    private float minAngle = 0.1f;
    public float angleReduce = 0.01f;

    private void AdjustHead()
    {
        /*
        Quaternion lookQuaternion = Quaternion.LookRotation(direction, curentParentTile.plane.normal);
        float t = (Time.time - startTime) / (HEADTURNTIME);

        // if new position has changed
        if (lookQuaternion != holdLookRot)
        {
            startTime = Time.time;
            startRot = head.transform.rotation;
        }

        if (head.transform.rotation != lookQuaternion)
        {
            head.transform.rotation = Quaternion.Lerp(startRot, lookQuaternion, t);
        }
        print(t);


        holdLookRot = lookQuaternion;
        */

        // Get angle between current head direction and aimed direction 
        float angle = Vector3.Angle(head.transform.forward, direction) * Mathf.Deg2Rad;
        float newAngle = angle * angleReduce;

        
        
        Quaternion lookQuaternion = Quaternion.LookRotation(direction, curentParentTile.plane.normal);

        

        float difference = angle - newAngle;
        print(difference);

        if (difference <= minAngle)
        {
            //head.transform.rotation = lookQuaternion;
        }

        if (difference > Mathf.PI)
        {
            head.transform.forward = Maths.RotateVectorXZ(head.transform.forward, difference * Time.deltaTime);
        }
        else
        {
            head.transform.forward = Maths.RotateVectorXZ(head.transform.forward, -difference * Time.deltaTime);
        }
        
    }

    /// <summary>
    /// Adjust the angle for each body part.
    /// Returns the new angle if necessary 
    /// </summary>
    /// <param name="angleMax"></param>
    /// <returns></returns>
    private void AdjustAngles(float angleMax)
    {
        for (int i = 0; i < bodyChain.Length; i++)
        {
            if (i + 1 < bodyChain.Length)
            {
                Vector3 normal = bodyChain[i + 1][0].position - bodyChain[i + 1][1].position;
                Vector3 newVec = bodyChain[i][0].position - bodyChain[i][1].position;

                // Checks if angle between two vectors is greater than angleMax 
                if (Mathf.Acos(Vector3.Dot(normal, newVec) / (Vector3.Magnitude(normal) * Vector3.Magnitude(newVec))) > angleMax)
                {
                    print("Out of range");
                }
            }
        }
    }

    private bool Move(float speed)
    {
        bool isMoving = false;
        // Turning 
        isMoving = Rotate(rotationSpeed);
        if(isMoving == false)
        {
            isMoving = DefaultMove(speed);
        }
        else
        {
            DefaultMove(speed);
        }
        

        return isMoving;
    }

    private bool DefaultMove(float speed)
    {
        bool isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            Vector3 newDir = Vector3.ProjectOnPlane(direction, curentParentTile.plane.normal);
            realSpeed += newDir * a * Time.deltaTime;

        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Backwards movement?
        }
        else
        {
            //head.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, curentParentTile.plane.normal), curentParentTile.plane.normal);

            // Slow down
            realSpeed -= realSpeed.normalized * a * Time.deltaTime;
        }


        if (realSpeed != Vector3.zero)
        {
            realSpeed = Vector3.ClampMagnitude(realSpeed, speed);
            //this.transform.position += realSpeed;

            // Check if position is not within any shape of current tile 
            Vector3 newPos = curentParentTile.MoveAlongTile(this.transform.position + realSpeed, this);

            if(newPos == Vector3.zero)
            {
                this.transform.position += realSpeed;
            }
            else
            {
                realSpeed = -realSpeed;
            }
            isMoving = true;
        }

        return isMoving;
    }

    private bool HoldMove(float speed, Plane plane) // Change to check for vector3.zero instead of bool 
    {
        bool isMoving = false;

        Vector3 moveAmount = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 newDir = Vector3.ProjectOnPlane(direction, curentParentTile.plane.normal);
            moveAmount += newDir * speed * Time.deltaTime;

        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector3 newDir = Vector3.ProjectOnPlane(-direction, curentParentTile.plane.normal);
            moveAmount += newDir * speed * Time.deltaTime;

            head.transform.rotation = Quaternion.LookRotation(direction, curentParentTile.plane.normal);
        }
        else
        {
            head.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }


        if (moveAmount != Vector3.zero)
        {
            this.transform.position += Vector3.ProjectOnPlane(moveAmount, plane.normal);
            pullable.position += Vector3.ProjectOnPlane(moveAmount, plane.normal);

            // Check if position is not within any shape of current tile 

            //this.transform.position = curentParentTile.MoveAlongTile(this.transform.position + moveAmount, this);
            isMoving = true;
        }

        return isMoving;
    }

    private bool Rotate(float speed)
    {
        bool isMoving = false;

        if (Input.GetKey(KeyCode.A))
        {
            // Rotation matrix
            direction = Maths.RotateVectorXZ(direction, speed * Time.deltaTime);
            isMoving = true;

            realSpeed = Vector3.ProjectOnPlane(realSpeed, new Plane(this.transform.position, this.transform.position + direction, this.transform.position + curentParentTile.plane.normal).normal);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Rotation matrix 
            direction = Maths.RotateVectorXZ(direction, -speed * Time.deltaTime);
            isMoving = true;

            realSpeed = Vector3.ProjectOnPlane(realSpeed, new Plane(this.transform.position, this.transform.position + direction, this.transform.position + curentParentTile.plane.normal).normal);
        }

        //head.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        return isMoving;
    }

    private bool Dash()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {

            return true;
        }

        return false;

    }

    private bool Hold()
    {
        if(Input.GetMouseButton(1))
        {
            // cast to see if hold anything 
            Collider[] collider = Physics.OverlapBox(grabPoint.position, Vector3.one, Quaternion.identity, grabbale);
            if(collider.Length > 0)
            {
                Handle handle = collider[0].GetComponent<Handle>();

                grabbedPlane = handle.Directionplane;
                pullable = handle.block;

                blockSparks = handle.sparksFX;

                return true;
            }
        }

        return false;
    }

    private void Spark()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(sparkFX, grabPoint.position, head.transform.rotation);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(this.transform.position, this.transform.position + direction);
        Gizmos.DrawCube(grabPoint.position, Vector3.one);
    }
}
