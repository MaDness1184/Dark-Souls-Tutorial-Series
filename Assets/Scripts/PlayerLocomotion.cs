using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DarkSoulsGame
{
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera; // camera not locked on

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        Vector3 normalVector;
        Vector3 targetPosition;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>(); // Use GetComponentInChildren<>() to get a componet of a child object
            cameraObject = Camera.main.transform; // Camera in scene that has the Tag -> MainCamera
            myTransform = transform;
            animatorHandler.Initialize();
        }

        public void Update()
        {
            float delta = Time.deltaTime;

            inputHandler.TickInput(delta); // update the movement input of the player every frame
            HandleMovement(delta);
            HandleRollingAndSprinting(delta);
        }

        #region Movement

        private void HandleRotation(float delta) // rotate player by delta time
        {
            Vector3 targetDir = Vector3.zero; // Reset target direction
            float moveOverride = inputHandler.moveAmount; 

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize(); // Normalize() Makes a vector have a magnitude of 1
            targetDir.y = 0; // Negating all y movement

            if (targetDir == Vector3.zero) // If no camera movement has been executed
                targetDir = myTransform.forward;

            float rs = rotationSpeed; // rotation sensitivity

            Quaternion tr = Quaternion.LookRotation(targetDir); // rotation needed to face toward the target direction
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // Spherical Interpolation: Rotate at a constant angular speed; Slerp(current rotation, end result rotation, speed of interpolation)

            myTransform.rotation = targetRotation;
        }

        public void HandleMovement(float delta)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical; // wherever the camera's Z-axis is facing * by the Y-INPUT of the user
            moveDirection += cameraObject.right * inputHandler.horizontal; // wherever the camera's Z-axis is facing * by the X-INPUT of the user
            moveDirection.Normalize(); // Clamping direction between 1 and 0; https://docs.unity3d.com/ScriptReference/Vector3.Normalize.html
            moveDirection.y = 0; // freeze movement on y-axis; stop levitation glitch

            float speed = movementSpeed;
            moveDirection *= speed; // direction x velocity

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector); // Keep the movement relative to what the player is standing on?
            rigidbody.velocity = projectedVelocity; // move the player

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            if (inputHandler.rollFlag) // If roll was preformed from InputManager
            {
                moveDirection = cameraObject.forward * inputHandler.vertical; // move player forward due to the camera's vertical direction
                moveDirection += cameraObject.right * inputHandler.horizontal; // add right movement due to the camera's horizontal direction

                if (inputHandler.moveAmount > 0) //If the Player has movement
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection); // https://docs.unity3d.com/ScriptReference/Quaternion.LookRotation.html
                    myTransform.rotation = rollRotation;
                }
                else // no movement = backstep
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

        #endregion
    }
}
