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

    public Vector3 direction;

    public Transform head;
    public Transform grabPoint;

    public LayerMask grabbale;
    private Plane grabbedPlane;
    private Transform pullable;
    private ParticleSystem blockSparks;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Snap Player to plane
        this.transform.position = curentParentTile.transform.position;

        direction = Vector3.ProjectOnPlane(Vector3.right, curentParentTile.plane.normal).normalized;

    }

    // Update is called once per frame
    void Update()
    {
        //direction = (Vector3.ProjectOnPlane(this.transform.position, Vector3.up) - Vector3.ProjectOnPlane(cam.position, Vector3.up)).normalized;

        switch (state)
        {
            case PlayerStates.Idle:
                // Play idle animation 

                if(Move(defaultSpeed))
                {
                    state = PlayerStates.DefaultMove;
                }

                if(Hold())
                {
                    state = PlayerStates.HoldIdle;
                }

                break;
            case PlayerStates.DefaultMove:

                if(!Move(defaultSpeed))
                {
                    state = PlayerStates.Idle;
                }

                if(Hold())
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

                if(!Hold())
                {
                    state = PlayerStates.Idle;
                }

                if(HoldMove(holdSpeed, grabbedPlane))
                {
                    state = PlayerStates.HoldMove;
                }

                break;
            case PlayerStates.HoldMove:

                if(!blockSparks.isPlaying)
                {
                    blockSparks.Play();
                }


                if (!HoldMove(holdSpeed, grabbedPlane))
                {
                    state = PlayerStates.HoldIdle;
                    blockSparks.Stop();
                }

                if(!Hold())
                {
                    state = PlayerStates.DefaultMove;
                    blockSparks.Stop();
                }


                break;
            default:
                break;
        }
    }

    private bool Move(float speed)
    {
        bool isMoving = false;
        // Turning 
        isMoving = Rotate(rotationSpeed) || DefaultMove(speed);

        return isMoving;
    }

    private bool DefaultMove(float speed)
    {
        bool isMoving = false;

        Vector3 moveAmount = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 newDir = Vector3.ProjectOnPlane(direction, curentParentTile.plane.normal);
            moveAmount += newDir * speed * Time.deltaTime;

            head.transform.rotation = Quaternion.LookRotation(newDir, curentParentTile.plane.normal);
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

        //print(curentParentTile.PlaneContainsPoint(Vector3.ProjectOnPlane(this.transform.position + moveAmount, curentParentTile.plane.normal)));

        if (moveAmount != Vector3.zero)
        {
            this.transform.position += moveAmount;

            // Check if position is not within any shape of current tile 

            //this.transform.position = curentParentTile.MoveAlongTile(this.transform.position + moveAmount, this);
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

            head.transform.rotation = Quaternion.LookRotation(newDir, curentParentTile.plane.normal);
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

        //print(curentParentTile.PlaneContainsPoint(Vector3.ProjectOnPlane(this.transform.position + moveAmount, curentParentTile.plane.normal)));

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
            direction = new Vector3(
                direction.x * Mathf.Cos(speed * Time.deltaTime) - direction.z * Mathf.Sin(speed * Time.deltaTime),
                0,
                direction.x * Mathf.Sin(speed * Time.deltaTime) + direction.z * Mathf.Cos(speed * Time.deltaTime));
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Rotation matrix 
            direction = new Vector3(
                direction.x * Mathf.Cos(-speed * Time.deltaTime) - direction.z * Mathf.Sin(-speed * Time.deltaTime),
                0,
                direction.x * Mathf.Sin(-speed * Time.deltaTime) + direction.z * Mathf.Cos(-speed * Time.deltaTime));
            isMoving = true;
        }

        head.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(this.transform.position, this.transform.position + direction);

    }
}
