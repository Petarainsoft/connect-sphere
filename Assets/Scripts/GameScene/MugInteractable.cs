using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace ConnectSphere
{
    public class MugInteractable : Interactable
    {
        [HideInInspector] [Networked] public bool IsActivated { get; set; }

        private Vector2 _initialPosition;
        private Transform _playerTransform;

        private void Awake()
        {
            _initialPosition = transform.position;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (!networkObject.HasStateAuthority)
                    return;
            }

            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                //playerObject.SetPickupData(InteractionCode, transform.position, gameObject);
                _playerTransform = playerObject.transform;
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (!networkObject.HasStateAuthority)
                    return;
            }

            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                //playerObject.SetPickupData(-1);
                _playerTransform = null;
            }
        }

        public override void Spawned()
        {
            base.Spawned();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void PickupRpc()
        {
            if (!IsActivated && _playerTransform != null)
            {
                transform.parent.SetParent(_playerTransform);
                transform.parent.localPosition = new Vector2(-0.4f, 0);
            }
            else
            {
                transform.parent.SetParent(null);
                transform.position = _initialPosition;
            }
        }
    }
}
