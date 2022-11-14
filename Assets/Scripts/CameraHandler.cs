using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform; // Lock on target
        public Transform cameraTransform; // transform of the camera
        public Transform cameraPivotTransform; // camera swivel for rotation
        private Transform myTransform; // transform of camera holder
        private Vector3 cameraTransformPosition; // 
        private LayerMask ignoreLayers;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minPivot = -35;
        public float maxPivot = 35;

        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); // Ignore some physics layers
        }

        public void FollowTarget(float delta) // Follow our target transform
        {
            Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed); // lerp between the Camera Holder position and the locked on target position / the Player's position; Lerp(a, b, t) = a + (b - a) * t; https://docs.unity3d.com/ScriptReference/Vector2.Lerp.html
            myTransform.position = targetPosition;
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput) // Handles camera rotation based on the Player's input every frame
        {
            lookAngle += (mouseXInput * lookSpeed) / delta; // (input direction * speed) / change in time; dividing by delta keeps the movement consistant
            pivotAngle -= (mouseYInput * pivotSpeed) / delta; // (input direction * speed) / change in time; dividing by delta keeps the movement consistant
            pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot); // Clamps the pivot value between minPivot and maxPivot; this clamps the rotation of the camera in the vertical direction to not clip through the floor or ceiling; https://docs.unity3d.com/ScriptReference/Mathf.Clamp.html

            Vector3 rotation = Vector3.zero; // reset rotation to get the raw lookAngle angular rotational value
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation); // https://docs.unity3d.com/ScriptReference/Quaternion.Euler.html
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero; // reset rotation to get the raw pivotAngle angular rotational value
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation); // https://docs.unity3d.com/ScriptReference/Quaternion.Euler.html
            cameraPivotTransform.localRotation = targetRotation; // https://docs.unity3d.com/ScriptReference/Transform-localRotation.html
        }
    }
}
