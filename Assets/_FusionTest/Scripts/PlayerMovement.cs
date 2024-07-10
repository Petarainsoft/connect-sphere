using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class PlayerMovement : NetworkBehaviour
    {
        public Camera Camera;

        private CharacterController _controller;

        public float PlayerSpeed = 2f;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public override void Spawned()
        {
            //if (HasStateAuthority)
            //{
            //    Camera = Camera.main;
            //    Camera.GetComponent<FirstPersonCamera>().Target = transform;
            //}
        }

        public override void FixedUpdateNetwork()
        {
            // Only move own player and not every other player. Each player controls its own player object.
            if (HasStateAuthority == false)
            {
                return;
            }

            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0) * Runner.DeltaTime * PlayerSpeed;

            _controller.Move(move);
        }
    }
}
