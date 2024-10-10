using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float force = 1f;
    Camera cam;
    Rigidbody rb;
    Vector3 movementDir;
    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        movementDir = (cam.transform.right * horizontalInput) + (cam.transform.forward * verticalInput);
        movementDir.y = 0f;

        movementDir.Normalize();


        rb.AddForce(movementDir * speed * Time.deltaTime, ForceMode.Acceleration);
    }
    void OnCollisionEnter(Collision collision)
    {
        if("Push" == collision.gameObject.tag){
            if(collision.gameObject.GetComponent<Rigidbody>() != null){
                collision.gameObject.GetComponent<Rigidbody>().AddForce(movementDir * force,ForceMode.Impulse);
            }
        }
        
    }
    void OnCollisionStay(Collision collision){
        if("Push" == collision.gameObject.tag){
            if(collision.gameObject.GetComponent<Rigidbody>() != null){
                collision.gameObject.GetComponent<Rigidbody>().AddForce(movementDir * force,ForceMode.Force);
            }
        }
    }
     void OnCollisionExit(Collision collision){
        if("Push" == collision.gameObject.tag){
        }
     }
}
