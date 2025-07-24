using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.Examples
{
    [RequireComponent(typeof(CharacterController))]
    public class TopDownPlayerController : MonoBehaviour
    {
        //Player object fields
        private CharacterController characterController;
        public Transform playerModel;
        public Vector2 input;

        // Movement field
        public float movementSpeed = 5f;
        public float accelerationSpeed = 10f;
        private Vector3 movementDirection;
        private Vector3 movementDirectionSmooth = Vector3.zero;

        // Rotation field
        public Vector3 targetRotation;
        public float rotationSpeed = 10f;
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

        private Vector3 cameraTargetPosition;
        private Vector3 cameraBasePositon;
        private Vector3 cameraZoomPosition;
        private Vector3 deltaCamera;

        // Bools
        private bool canMove = true;
        private bool isMoving = false;


        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            deltaCamera = cameraTransform.position - transform.position;
            cameraTargetPosition = transform.position;
            cameraTransform.parent = null;
        }

        private void Update()
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            //Calculate direction
            float moveHorizontal = input.x * movementSpeed;
            float moveVertical = input.y * movementSpeed;

            //I want to make the movement smooth, so...
            movementDirection = (fwd * moveVertical) + (right * moveHorizontal);
            movementDirectionSmooth = Vector3.Lerp(movementDirectionSmooth, movementDirection, movementSpeed*Time.deltaTime);

            isMoving = movementDirection != Vector3.zero;
            
            //Move
            characterController.Move(movementDirectionSmooth * Time.deltaTime);

            //Rotation engine
            Vector3 forwardPos = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
            Vector3 rightPos = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z);
            Vector3 inputDir = forwardPos * input.x + rightPos * -input.y;
            
                targetRotation = inputDir;
            
            playerModel.forward = Vector3.Slerp(playerModel.forward, inputDir.normalized, rotationSpeed * Time.deltaTime);

            

            /*float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
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
            }*/

        }

        private void FixedUpdate()
        {
            
            /*rb.linearVelocity = movementDirection * movementSpeed;

            cameraBasePositon = Vector3.Lerp(cameraBasePositon, cameraTargetPosition, cameraSmoothSpeed * Time.deltaTime);
            cameraTransform.position = cameraBasePositon + cameraZoomPosition;

            if (isMoving)
                rb.MoveRotation(Quaternion.Normalize(targetRotation));
            else
                rb.MoveRotation(transform.rotation);
            */
        }
    }
}