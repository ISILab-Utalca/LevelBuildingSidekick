using System;
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
        public Animator modelAnimator;

        // Movement field
        public float movementSpeed = 5f;
        public float accelerationSpeed = 10f;
        private Vector3 movementDirection;
        private Vector3 movementDirectionSmooth = Vector3.zero;

        // Rotation field
        private Vector3 facingDirection = Vector3.zero;
        private Vector3 targetRotation;
        public float rotationSpeed = 10f;
        public float idleRotationSpeed = 5f;

        // Camera fields
        public Transform cameraTransform;
        public float cameraFollowDistanceMoving = 2.5f;
        public float cameraFollowDistanceIdle = 0f;
        public float cameraSmoothSpeed = 5f;
        public float minZoomDistance = 2f;
        public float maxZoomDistance = 10f;
        public float zoomSpeed = 5f;
        public float zoomDistance = 5f;

        private Vector3 cameraTargetPosition;
        private Vector3 cameraBasePosition;
        private Vector3 cameraZoomPosition;
        private Vector3 deltaCamera;

        // Bools
        private bool canMove = true;
        public bool isMoving = false;


        private void Start()
        {
            characterController = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            deltaCamera = cameraTransform.position - playerModel.position;
            cameraTargetPosition = playerModel.position + deltaCamera;
            cameraBasePosition = cameraTargetPosition;
        }

        private void Update()
        {
            UpdateMovement();
            UpdateCamera();

        }

        private void UpdateMovement()
        {
            //Input
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            isMoving = input != Vector2.zero;

            Vector3 forwardPos = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
            Vector3 rightPos = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z);

            //Calculate direction
            float moveHorizontal = input.x * movementSpeed;
            float moveVertical = input.y * movementSpeed;

            //I want to make the movement smooth, so...
            movementDirection = (forwardPos * moveVertical) + (rightPos * moveHorizontal);
            movementDirectionSmooth = Vector3.Lerp(movementDirectionSmooth, movementDirection, accelerationSpeed * Time.deltaTime);

            //Move
            characterController.Move(movementDirectionSmooth * Time.deltaTime);

            //To whoever is seeing this abomination of a code in the near future, these conditions are here so the model's facing direction MOSTLY
            //prioritizes the player's input. HOWEVER, it also leaves a bigger time window to leave the model facing in diagonal, since
            //it'll temporarily prioritize the already existing facing direction before reading your input again.
            //No movement lag, but some time to look in diagonal! -Alice
            facingDirection.x = Mathf.Abs(movementDirectionSmooth.x) > movementSpeed / 2 ? input.x != 0 ? input.x : Mathf.Sign(movementDirectionSmooth.normalized.x) : input.x;
            facingDirection.z = Mathf.Abs(movementDirectionSmooth.z) > movementSpeed / 2 ? input.y != 0 ? input.y : Mathf.Sign(movementDirectionSmooth.normalized.z) : input.y;
            Vector3 inputDir = forwardPos * facingDirection.z + rightPos * facingDirection.x;

            //This conditional allows the model to always snap to the facing direction. It stops spinning if it registers a 0, so it has to be this way.
            if (input != Vector2.zero)
            {
                
                targetRotation = inputDir;
            }
            playerModel.forward = Vector3.Slerp(playerModel.forward, targetRotation, rotationSpeed * Time.deltaTime);
            modelAnimator.SetFloat("MoveBlend", movementDirectionSmooth.magnitude/movementSpeed);

        }
        
        private void UpdateCamera()
        {
            //Zoom tech (not used for now)

            /*float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            zoomDistance -= scrollWheel * zoomSpeed;
            zoomDistance = Mathf.Clamp(zoomDistance, minZoomDistance, maxZoomDistance);
            cameraZoomPosition = new Vector3(0f, zoomDistance, -zoomDistance);*/

            cameraTargetPosition = isMoving
                ? playerModel.position + deltaCamera + (facingDirection * cameraFollowDistanceMoving)
                : playerModel.position + deltaCamera + (facingDirection * cameraFollowDistanceIdle);
            //Update camera position
            cameraBasePosition = Vector3.Lerp(cameraBasePosition, cameraTargetPosition, cameraSmoothSpeed * Time.deltaTime);
            cameraTransform.position = cameraBasePosition + cameraZoomPosition;
        }
    }
}