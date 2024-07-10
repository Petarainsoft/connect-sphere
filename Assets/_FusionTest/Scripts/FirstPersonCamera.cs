using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class FirstPersonCamera : MonoBehaviour
    {
        public Transform Target;
        public float MouseSensitivity = 10f;

        private float verticalRotation;
        private float horizontalRotation;

        void LateUpdate()
        {
            if (Target == null)
            {
                return;
            }
            transform.position = new Vector3(Target.position.x, transform.position.y, Target.position.z);
        }
    }
}
