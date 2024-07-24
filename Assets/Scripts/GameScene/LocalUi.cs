using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class LocalUi : MonoBehaviour
    {
        public GameObject InteractPrompt;

        public static Action OnTriggerInteraction;

        private void OnEnable()
        {
            OnTriggerInteraction += TogglePrompt;
        }

        private void OnDisable()
        {
            OnTriggerInteraction -= TogglePrompt;
        }

        private void TogglePrompt()
        {
            InteractPrompt.SetActive(!InteractPrompt.activeSelf);
        }
    }
}
