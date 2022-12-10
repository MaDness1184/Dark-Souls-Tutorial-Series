using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkSoulsGame
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool b_Input; // input of button b
        public bool rollFlag;
        public bool isInteracting;

        PlayerControls inputActions;
        CameraHandler cameraHandler;

        Vector2 movementInput; // Vector2 to store InputAction -> Movement's value
        Vector2 cameraInput; // Vector2 to store InputAction -> Camera's value

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

        public void OnEnable() //  Assign delegated funtions to InputActions, Enable Player Input
        {
            if (inputActions == null) // If Player object has no input actions referenced
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>(); // Adding value read from Movement InputAction using delagate-lambda function 
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>(); // i = context, where cameraInput is storing the value from the Camera InputAction using delagate-lambda function 
            }

            inputActions.Enable();
        }

        private void OnDiable() // Disable Player Input
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x; // Storing the X-axis of the INPUT not the worldspace
            vertical = movementInput.y; // Storing the Y-axis of the INPUT not the worldspace
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // Clamp01 clamps the value between 0 and 1; 
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput(float delta)
        {
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed; // Checks if the Roll action was preformed from the inputSystem

            if (b_Input)
            {
                rollFlag = true;
            }
        }
    }
}
