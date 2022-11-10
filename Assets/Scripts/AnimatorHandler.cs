using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator anim;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical"); // Turn "Vertical" parameter of the animator to a id
            horizontal = Animator.StringToHash("Horizontal"); // Turn "Vertical" parameter of the animator to a id
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement) // moveAmount in InputHandler is passed
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

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime); // SetFloat(string name, value, dampTime, deltaTime)
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime); // SetFloat(string name, value, dampTime, deltaTime)
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }
    }
}