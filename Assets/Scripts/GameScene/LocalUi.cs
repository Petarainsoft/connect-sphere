using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class LocalUi : MonoBehaviour
    {
        [Header("UI")]
        public GameObject InteractPrompt;
        public GameObject UserInformation;

        [Header("Events")]
        [SerializeField] private VoidEventHandlerSO _onInteractionTriggered;
        [SerializeField] private VoidEventHandlerSO _onOpenUserInfoButtonPressed;
        [SerializeField] private BooleanEventHandlerSO _onUiInteracting;

        private void OnEnable()
        {
            _onInteractionTriggered.OnEventRaised += TogglePrompt;
            _onOpenUserInfoButtonPressed.OnEventRaised += ToggleUserInfo;
        }

        private void OnDisable()
        {
            _onInteractionTriggered.OnEventRaised -= TogglePrompt;
            _onOpenUserInfoButtonPressed.OnEventRaised -= ToggleUserInfo;
        }

        private void TogglePrompt()
        {
            InteractPrompt.SetActive(!InteractPrompt.activeSelf);
        }

        public void ToggleUserInfo()
        {
            UserInformation.SetActive(!UserInformation.activeSelf);
            _onUiInteracting.RaiseEvent(UserInformation.activeSelf);
        }
    }
}
