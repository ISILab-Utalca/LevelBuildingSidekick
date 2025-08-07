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
        private ShootingController shootingController;
        public Transform playerModel;
        public Animator modelAnimator;

        // Movement field
        [Header("Movement Fields")]
        public float movementSpeed = 5f;
        public float dashingSpeed = 15f;
        public float shootingSpeed = 2.5f;
        private float targetSpeed;
        public float accelerationSpeed = 20f;

        private Vector3 movementDirection;
        private Vector3 movementDirectionSmooth = Vector3.zero;

        // Rotation field
        [Header("Rotation Fields")]
        public float rotationSpeed = 10f;
        public float idleRotationSpeed = 5f;
        private Vector3 facingDirection = Vector3.zero;
        private Vector3 targetRotation;

        // Camera fields
        [Header("Camera Fields")]
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

        private float dashTimer = 0f;
        private float shootingTimer = 0f;

        // Bools
        private bool canMove = true;
        private bool isMoving = false;

        public enum playerState { FreeRoam, Dashing, Shooting };
        protected playerState currentState;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            shootingController = GetComponent<ShootingController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //We don't need this!
            var cameraObj = GameObject.FindWithTag("MainCamera");
            if (cameraObj!=null)
            {
                Destroy(cameraObj);
            }

            deltaCamera = cameraTransform.position - playerModel.position;
            cameraTargetPosition = playerModel.position + deltaCamera;
            cameraBasePosition = cameraTargetPosition;

            currentState = playerState.FreeRoam;
        }

        private void Update()
        {
            //Timer stuff
            if (currentState != playerState.Dashing) dashTimer = UpdateTimer(dashTimer);
            if (currentState != playerState.Shooting) shootingTimer = UpdateTimer(shootingTimer);

            //Movement and controller stuff
            UpdateCamera();
            switch (currentState)
            {
                case playerState.FreeRoam:
                    targetSpeed = movementSpeed;
                    CheckMovementInput();

                    //Dash button
                    if (Input.GetButtonDown("Fire2") && dashTimer == 0) StartCoroutine(Dash());
                    //Shoot button
                    if (Input.GetButton("Fire1") && shootingTimer == 0) StartCoroutine(Shoot());
                    break;
                case playerState.Dashing:
                    targetSpeed = dashingSpeed;
                    movementDirection = playerModel.forward * dashingSpeed;
                    break;
                case playerState.Shooting:
                    targetSpeed = shootingSpeed;
                    CheckMovementInput();
                    break;
            }
            UpdateMovement();

        }

        private void CheckMovementInput()
        {
            //Input
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            isMoving = input != Vector2.zero;

            var forward = new Vector3(transform.forward.x, 0, transform.forward.z);
            var right = new Vector3(transform.right.x, 0, transform.right.z);
            var inputDirection = (forward * input.y) + (right * input.x);

            //Calculate direction
            float moveHorizontal = input.x * targetSpeed;
            float moveVertical = input.y * targetSpeed;

            //I want to make the movement smooth, so...
            movementDirection = inputDirection * targetSpeed;

            //To whoever is seeing this abomination of a code in the near future, these conditions are here so the model's facing direction MOSTLY
            //prioritizes the player's input. HOWEVER, it also leaves a bigger time window to leave the model facing in diagonal, since
            //it'll temporarily prioritize the already existing facing direction before reading your input again.
            //No movement lag, but some time to look in diagonal! -Alice
            
            facingDirection.x = Mathf.Abs(movementDirectionSmooth.x) > targetSpeed * 0.025 ? inputDirection.x != 0 ? inputDirection.x : Mathf.Sign(movementDirectionSmooth.normalized.x) : inputDirection.x;
            facingDirection.z = Mathf.Abs(movementDirectionSmooth.z) > targetSpeed * 0.025 ? inputDirection.z != 0 ? inputDirection.z : Mathf.Sign(movementDirectionSmooth.normalized.z) : inputDirection.z;

        }
        
        private void UpdateMovement()
        {
            movementDirectionSmooth = Vector3.Lerp(movementDirectionSmooth, movementDirection, accelerationSpeed * Time.deltaTime);
            characterController.Move(movementDirectionSmooth * Time.deltaTime);

            //Rotation control
            //This conditional allows the model to always snap to the facing direction. It stops spinning if it registers a 0, so it has to be this way.
            if (isMoving)
            {
                targetRotation = facingDirection;
            }
            playerModel.forward = Vector3.Slerp(playerModel.forward, targetRotation, rotationSpeed * Time.deltaTime);
            modelAnimator.SetFloat("MoveBlend", movementDirectionSmooth.magnitude / movementSpeed);
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

        IEnumerator Dash()
        {
            dashTimer = 0.5f;

            //Direction correction
            playerModel.forward = targetRotation;
            movementDirection = playerModel.forward;

            currentState = playerState.Dashing;
            modelAnimator.SetTrigger("Dash");

            yield return new WaitForSeconds(0.20f);
            currentState = playerState.FreeRoam;
        }

        IEnumerator Shoot()
        {
            shootingTimer = 0.2f;

            //Direction correction
            playerModel.forward = targetRotation;
            movementDirection = playerModel.forward;

            currentState = playerState.Shooting;
            modelAnimator.SetBool("Shooting", true);

            shootingController.Shoot();

            yield return new WaitForSeconds(0.20f);

            if (Input.GetButton("Fire1")) StartCoroutine(Shoot());
            else
            {
                modelAnimator.SetBool("Shooting", false);
                currentState = playerState.FreeRoam;
            }
        }

        private float UpdateTimer(float timer)
        {
            if (timer == 0) return timer;
            timer = timer-Time.deltaTime > 0 ? timer-Time.deltaTime : 0;
            return timer;
        }
    }
}