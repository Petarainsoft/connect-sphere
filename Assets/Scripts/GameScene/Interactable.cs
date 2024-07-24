using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class Interactable : NetworkBehaviour
    {
        public int InteractionCode;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (!networkObject.HasStateAuthority)
                    return;
            }    

            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                playerObject.SetInteractionData(InteractionCode, transform.position, gameObject);
                LocalUi.OnTriggerInteraction?.Invoke();
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (!networkObject.HasStateAuthority)
                    return;
            }

            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                playerObject.SetInteractionData(-1);
                LocalUi.OnTriggerInteraction?.Invoke();
            }
        }
    }
}
