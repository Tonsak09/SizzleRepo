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

    public Transform head;


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
        if (Input.GetKey(KeyCode.A))
        {
            direction = new Vector3(
                direction.x * Mathf.Cos(speed * Time.deltaTime) - direction.z * Mathf.Sin(speed * Time.deltaTime),
                0,
                direction.x * Mathf.Sin(speed * Time.deltaTime) + direction.z * Mathf.Cos(speed * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = new Vector3(
                direction.x * Mathf.Cos(-speed * Time.deltaTime) - direction.z * Mathf.Sin(-speed * Time.deltaTime),
                0,
                direction.x * Mathf.Sin(-speed * Time.deltaTime) + direction.z * Mathf.Cos(-speed * Time.deltaTime));
        }
       

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

            head.transform.rotation = Quaternion.LookRotation(newDir, curentParentTile.plane.normal);
        }
        else
        {
            head.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        print(curentParentTile.PlaneContainsPoint(Vector3.ProjectOnPlane(this.transform.position + moveAmount, curentParentTile.plane.normal)));

        if (moveAmount != Vector3.zero)
        {
            this.transform.position += moveAmount;

            // Check if position is not within any shape of current tile 

            //this.transform.position = curentParentTile.MoveAlongTile(this.transform.position + moveAmount, this);
        }

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(this.transform.position, this.transform.position + direction);
    }
}
