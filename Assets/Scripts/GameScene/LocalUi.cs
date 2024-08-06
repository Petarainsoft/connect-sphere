using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class LocalUi : Singleton<MonoBehaviour>
    {
        [Header("UI")]
        public GameObject InteractPrompt;
        public GameObject UserInformation;

        [Header("Events")]
        [SerializeField] private VoidEventHandlerSO _onInteractionTriggered;
        [SerializeField] private VoidEventHandlerSO _onOpenUserInfoButtonPressed;
        [SerializeField] private BooleanEventHandlerSO _onUiInteracting;

        [Header("Prefabs")]
        [SerializeField] private GameObject _interactPromptPrefab;
        [SerializeField] private GameObject _userInformationPrefab;
        [SerializeField] private ActivityController _rocPapSciPrefab;

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
            if (InteractPrompt == null)
            {
                InteractPrompt = Instantiate(_interactPromptPrefab, transform);
            }
            else
            {
                InteractPrompt.SetActive(!InteractPrompt.activeSelf);
            }
        }

        public void ToggleUserInfo()
        {
            if (UserInformation == null)
            {
                UserInformation = Instantiate(_userInformationPrefab, transform);
                _onUiInteracting.RaiseEvent(UserInformation.activeSelf);
            }
            else
            {
                UserInformation.SetActive(!UserInformation.activeSelf);
                _onUiInteracting.RaiseEvent(UserInformation.activeSelf);
            }
        }

        public PopupHandler CreatePopup()
        {
            var asset = Resources.Load<PopupHandler>("Popup/Popup_Blue");
            PopupHandler popup = Instantiate(asset);
            return popup;
        }
    }
}
