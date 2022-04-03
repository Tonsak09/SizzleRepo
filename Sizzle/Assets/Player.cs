using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public QuadNode curentParentTile;

    public Transform cam;

    public float speed;
    public Vector3 direction;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        direction = (Vector3.ProjectOnPlane(this.transform.position, Vector3.up) - Vector3.ProjectOnPlane(cam.position, Vector3.up)).normalized;

        Move();
    }

    private void Move()
    {
        if(Input.GetKey(KeyCode.W))
        {
            Vector3 newDir = Vector3.ProjectOnPlane(direction, curentParentTile.plane.normal).normalized;
            print(newDir);
            this.transform.position += newDir * speed * Time.deltaTime;
        }
    }
}
