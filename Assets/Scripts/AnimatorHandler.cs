using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkSoulsGame
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator anim;
        public InputHandler inputHandler;
        public PlayerLocomotion playerLocomotion;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize() // Initialize gameObject if method called
        {
            anim = GetComponent<Animator>();
            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical"); // Turn "Vertical" parameter of the animator to a id
            horizontal = Animator.StringToHash("Horizontal"); // Turn "Vertical" parameter of the animator to a id
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting) // moveAmount in InputHandler is passed
        {
            #region Vertical
            ///
            /// Clamping vertical values
            ///
            float v = 0;
            if (verticalMovement > 0 && verticalMovement < 0.55f)
                v = 0.5f;                                           // Value to trigger run animation
            else if (verticalMovement > 0.55f)
                v = 1;                                              // Value to trigger sprint animation 
            else if (verticalMovement < 0 && verticalMovement < -0.55f)
                v = -0.5f;                                          // Value to trigger reverse run animation? 
            else if (verticalMovement < -0.55f)
                v = -1;                                             // Value to trigger reverse sprint animation?                                        
            else
                v = 0;                                              // Value to trigger idle animation

            #endregion

            #region Horizontal
            ///
            /// Clamping horizontal values
            ///
            float h = 0;
            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
                h = 0.5f;
            else if (horizontalMovement > 0.55f)
                h = 1;
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
                h = -0.5f;
            else if (horizontalMovement < -0.55f)
                h = 1;
            else
                h = 0;

            #endregion

            if (isSprinting) // if flag isSprinting in PlayerLocomotion script is true then set v = 2 to trigger sprint animation
            {
                v = 2;
                h = horizontalMovement;
            }

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime); // SetFloat(string name, value, dampTime, deltaTime)
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime); // SetFloat(string name, value, dampTime, deltaTime)
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting) // Play a target animation from the animator on the entity; if the entity is interacting apply root motion
        {
            anim.applyRootMotion = isInteracting; // The animation will only have root motion applied if isInteracting is true; root motion is a bool that decides whether the character's animation will take over movement; https://docs.unity3d.com/ScriptReference/Animator-applyRootMotion.html
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f); // Play the target animation with the animation and transition duration passed in; Animator.CrossFade(string stateName, float normalizedTransitionDuration); https://docs.unity3d.com/ScriptReference/Animator.CrossFade.html
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }

        private void OnAnimatorMove()
        {
            if (inputHandler.isInteracting == false)
                return;

            float delta = Time.deltaTime;
            playerLocomotion.rigidbody.drag = 0; // used to slow down and object
            Vector3 deltaPosition = anim.deltaPosition; // Animator's position
            deltaPosition.y = 0; // used to prevent bugs; no funky animations
            Vector3 velocity = deltaPosition / delta; // velocity = position / time
            playerLocomotion.rigidbody.velocity = velocity;
        }
    }
}