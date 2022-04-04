using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public QuadNode curentParentTile;

    public Transform cam;

    public float rotationSpeed;
    public float speed;
    public Vector3 direction;


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

        Move();
    }

    private void Move()
    {
        // Rotation
        if (Input.GetKey(KeyCode.D))
        {
            direction += Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, curentParentTile.plane.normal).eulerAngles;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction -= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, curentParentTile.plane.normal).eulerAngles;
        }

        Vector3 moveAmount = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
        {
            Vector3 newDir = direction.normalized;
            moveAmount += newDir * speed * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            Vector3 newDir =-direction.normalized;
            moveAmount += newDir * speed * Time.deltaTime;
        }

        

        if(moveAmount != Vector3.zero)
        {
            this.transform.position += moveAmount;

            // Check if position is not within any shape of current tile 

            //this.transform.position = curentParentTile.MoveAlongTile(this.transform.position + moveAmount, this);
        }
    }
}
