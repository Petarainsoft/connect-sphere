using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace ConnectSphere
{
    public class SelectionCanvasControl : Singleton<SelectionCanvasControl>
    {
        [Header("UI")]
        [SerializeField] private Transform _avatarsContainer;
        [SerializeField] private TMP_InputField _inputPlayerName;
        [SerializeField] private Button _buttonStart;

        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfoSo;
        [SerializeField] public string InGameSceneName = "GameScene";

        private int _selectedIndex = 0;

        public Action<int> OnAvatarImageClicked;

        private void OnEnable()
        {
            OnAvatarImageClicked += HandleSelectedAvatar;
            _buttonStart.onClick.AddListener(StartGame);
        }

        private void OnDisable()
        {
            OnAvatarImageClicked -= HandleSelectedAvatar;
            _buttonStart.onClick.RemoveListener(StartGame);
        }

        private void HandleSelectedAvatar(int index)
        {
            foreach (Transform child in _avatarsContainer)
            {
                if (child.GetSiblingIndex() == index)
                {
                    child.GetComponent<Image>().enabled = true;
                    _selectedIndex = index;
                }
                else
                {
                    child.GetComponent<Image>().enabled = false;
                }
            }    
        }

        private void StartGame()
        {
            _playerInfoSo.PlayerName = _inputPlayerName.text.Trim();
            _playerInfoSo.AvatarIndex = _selectedIndex;
            GoToInGame();
        }

        private void GoToInGame()
        {
            if (_playerInfoSo.Role == NetworkRole.Host)
            {
                if (NetworkManager.Singleton.StartHost())
                {
                    SceneTransitionHandler.sceneTransitionHandler.RegisterCallbacks();
                    SceneTransitionHandler.sceneTransitionHandler.SwitchScene(InGameSceneName);
                }
                else
                {
                    Debug.LogError("Failed to start host.");
                }
            }

            if (_playerInfoSo.Role == NetworkRole.Server)
            {
                if (NetworkManager.Singleton.StartServer())
                {
                    SceneTransitionHandler.sceneTransitionHandler.SwitchScene(InGameSceneName);
                }
            }

            if (_playerInfoSo.Role == NetworkRole.Client)
            {
                if (NetworkManager.Singleton.StartClient())
                {
                    SceneTransitionHandler.sceneTransitionHandler.SwitchScene(InGameSceneName);
                }
                else
                {
                    Debug.LogError("Failed to start client.");
                }
            }
        }
    }
}
 