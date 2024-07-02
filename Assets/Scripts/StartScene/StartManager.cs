using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ConnectSphere
{
    public class StartManager : Singleton<StartManager>
    {
        [SerializeField] private Transform _avatarsContainer;
        [SerializeField] private TMP_InputField _inputPlayerName;
        [SerializeField] private Button _buttonStart;
        [SerializeField] private PlayerInfoSO _playerInfoSo;

        private int _selectedIndex = 0;
        private readonly string GAME_SCENE = "GameScene";

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
            SceneManager.LoadSceneAsync(GAME_SCENE);
        }
    }
}
 