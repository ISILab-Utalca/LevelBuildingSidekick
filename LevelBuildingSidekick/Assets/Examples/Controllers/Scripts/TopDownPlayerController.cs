using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPlayerController : MonoBehaviour
{
    // Movement field
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;

    // Rotation field
    public float idleRotationSpeed = 5f;
    public float rotationThreshold = 0.01f;

    // Camera fields
    public Transform cameraTransform;
    public float cameraFollowDistanceMoving = 5f;
    public float cameraFollowDistanceIdle = 0f;
    public float cameraSmoothSpeed = 5f;
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 10f;
    public float zoomSpeed = 5f;
    public float zoomDistance = 5f;

    private Rigidbody rb;
    private Quaternion targetRotation;
    private Vector3 movementDirection;
    private Vector3 cameraTargetPosition;
    private Vector3 cameraBasePositon;
    private Vector3 cameraZoomPosition;
    private bool isMoving = false;
    private Vector3 deltaCamera;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        targetRotation = transform.rotation;
        deltaCamera = cameraTransform.position - transform.position;
        cameraTargetPosition = transform.position;
        cameraTransform.parent = null;
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
            targetRotation = Quaternion.Lerp(targetRotation, toRotation, rotationSpeed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        zoomDistance -= scrollWheel * zoomSpeed;
        zoomDistance = Mathf.Clamp(zoomDistance, minZoomDistance, maxZoomDistance);
        cameraZoomPosition = new Vector3(0f, zoomDistance, -zoomDistance);

        if (isMoving)
        {
            cameraTargetPosition = transform.position + deltaCamera + (transform.forward * cameraFollowDistanceMoving);
        }
        else
        {
            cameraTargetPosition = transform.position + deltaCamera + (transform.forward * cameraFollowDistanceIdle);
        }
    }

    private void FixedUpdate()
    {
        // Aplicar movimiento al jugador
        rb.velocity = movementDirection * movementSpeed;

        // Mover la cámara suavemente hacia la posición objetivo
        cameraBasePositon = Vector3.Lerp(cameraBasePositon, cameraTargetPosition, cameraSmoothSpeed * Time.deltaTime);
        cameraTransform.position = cameraBasePositon + cameraZoomPosition;

        if (isMoving)
            rb.MoveRotation(Quaternion.Normalize(targetRotation));
        else
            rb.MoveRotation(transform.rotation);
    }
}
