using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class Interactable : NetworkBehaviour
    {
        public int InteractionCode;
        [SerializeField] protected GameObject _highlightSprite;
        [SerializeField] protected VoidEventHandlerSO _onInteractionTriggered;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (!networkObject.HasStateAuthority)
                    return;
            }    

            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                ToggleHighlight(true);
                playerObject.SetInteractionData(InteractionCode, transform.position, this);
                _onInteractionTriggered.RaiseEvent();
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
                ToggleHighlight(false);
                playerObject.SetInteractionData(-1);
                _onInteractionTriggered.RaiseEvent();
            }
        }

        public void ToggleHighlight(bool value)
        {
            if (_highlightSprite != null)
            {
                _highlightSprite.SetActive(value);
            }
        }
    }
}
