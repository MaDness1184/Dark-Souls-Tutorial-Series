using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RC
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

            moveDirection = cameraObject.forward * inputHandler.vertical; // wherever the camera's Z-axis is facing * by the Y-INPUT of the user
            moveDirection += cameraObject.right * inputHandler.horizontal; // wherever the camera's Z-axis is facing * by the X-INPUT of the user
            moveDirection.Normalize(); // Clamping direction between 1 and 0

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

        #endregion
    }
}
