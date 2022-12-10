using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DarkSoulsGame
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform; // Lock on target
        public Transform cameraTransform; // transform of the camera
        public Transform cameraPivotTransform; // camera swivel for rotation
        private Transform myTransform; // transform of camera holder
        private Vector3 cameraTransformPosition;
        private Vector3 cameraFollowVelocity = Vector3.zero;
        private LayerMask ignoreLayers;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minCollisionOffset = 0.2f;

        private float targetPosition;
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
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed); // lerp between the Camera Holder position and the locked on target position / the Player's position; Lerp(a, b, t) = a + (b - a) * t; https://docs.unity3d.com/ScriptReference/Vector2.Lerp.html
            myTransform.position = targetPosition;

            HandleCameraCollisions(delta);
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

        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition; // Where the camera started at the beginning of the scene
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers)) // if the raycast has hit a collider in the list of physics layers; hitSphereCast(origin, radius, direction(opposite sweep to where the camera is facing), hitInfo(info of the obj hit), maxDistance, layermask(ignore these physics layers), queryTrigger); https://docs.unity3d.com/ScriptReference/Physics.SphereCast.html
            {
                float dist = Vector3.Distance(cameraPivotTransform.position, hit.point); // holds the distance of the camera's position and the raycast's point of contact; Vector3.Distance(distance a, distance b); https://docs.unity3d.com/ScriptReference/Vector3.Distance.html
                targetPosition = -(dist - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minCollisionOffset)
            {
                targetPosition = -minCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f); // Lerp to target position over delta; https://docs.unity3d.com/ScriptReference/Mathf.Lerp.html
            cameraTransform.localPosition = cameraTransformPosition; // change the local position to the camera's new position
        }
    }
}
