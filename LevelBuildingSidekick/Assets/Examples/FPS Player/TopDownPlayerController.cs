using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 10f;
    private float zoomDistance = 5f;

    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 movementDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Movimiento del jugador
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movementDirection = cameraTransform.forward * moveVertical + cameraTransform.right * moveHorizontal;
        movementDirection.y = 0f;
        movementDirection.Normalize();

        // Rotación del jugador
        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        zoomDistance -= scrollWheel * zoomSpeed;
        zoomDistance = Mathf.Clamp(zoomDistance, minZoomDistance, maxZoomDistance);
        cameraTransform.localPosition = new Vector3(0f, zoomDistance, -zoomDistance);
    }

    private void FixedUpdate()
    {
        // Aplicar movimiento al jugador
        rb.velocity = movementDirection * movementSpeed;
    }
}
