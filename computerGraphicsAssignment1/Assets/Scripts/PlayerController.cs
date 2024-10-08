using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    Camera cam;
    Rigidbody rb;

    private void Start()
    {
         cam = Camera.main;
         rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movementDir = (cam.transform.right * horizontalInput) + (cam.transform.forward * verticalInput); 
        movementDir.y = 0f;

        movementDir.Normalize();


        rb.AddForce(movementDir * speed * Time.deltaTime, ForceMode.Acceleration);
    }
}
