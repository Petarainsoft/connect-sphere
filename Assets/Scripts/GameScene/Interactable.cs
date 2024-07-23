using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class Interactable : MonoBehaviour
    {
        public int InteractionCode;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                playerObject.SetInteractionCode(InteractionCode, transform.position);
                LocalUi.OnTriggerInteraction?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                playerObject.SetInteractionCode(-1);
                LocalUi.OnTriggerInteraction?.Invoke();
            }
        }
    }
}
