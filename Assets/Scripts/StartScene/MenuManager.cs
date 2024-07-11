using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

namespace ConnectSphere
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private NetworkRunner _networkRunnerPrefab;

        [Header("Network Canvas")]
        [SerializeField] private TMP_InputField _inputRoomName;
        [SerializeField] private TextMeshProUGUI _roomNamePlaceholder;

        [Header("Selection Canvas")]
        [SerializeField] private TMP_InputField _inputPlayerName;
        [SerializeField] private TextMeshProUGUI _playerNamePlaceholder;
        [SerializeField] private Transform _avatarsContainer;
        [SerializeField] private TextMeshProUGUI _textConnectionStatus;
        [SerializeField] private Button _buttonStart;

        [Header("Others")]
        [SerializeField] GameObject _loadingCanvas;
        [SerializeField] GameObject _networkCanvasObject;
        [SerializeField] GameObject _selectionCanvasObject;
        [SerializeField] private PlayerInfoSO _playerInfoSo;
        [SerializeField] private string _gameScenePath;

        private NetworkRunner _runnerInstance;
        private string _tempRoomName;
        private string _tempPlayerName;
        private int _selectedAvatarIndex = 0;

        public static Action<int> OnAvatarImageClicked;

        public string RoomName => _tempRoomName;

        private void OnEnable()
        {
            OnAvatarImageClicked += HandleSelectedAvatar;
        }

        private void OnDisable()
        {
            OnAvatarImageClicked -= HandleSelectedAvatar;
        }

        public void OnJoinButtonClicked()
        {
            if (string.IsNullOrEmpty(_inputRoomName.text.Trim()))
            {
                _tempRoomName = _roomNamePlaceholder.text;
            }
            else
            {
                _tempRoomName = _inputRoomName.text;
            }

            _networkCanvasObject.SetActive(false);
            _selectionCanvasObject.SetActive(true);
        }

        private void HandleSelectedAvatar(int index)
        {
            foreach (Transform child in _avatarsContainer)
            {
                if (child.GetSiblingIndex() == index)
                {
                    child.GetComponent<Image>().enabled = true;
                    _selectedAvatarIndex = index;
                }
                else
                {
                    child.GetComponent<Image>().enabled = false;
                }
            }
        }

        public void OnStartButtonClicked()
        {
            if (string.IsNullOrEmpty(_inputPlayerName.text.Trim()))
            {
                _tempPlayerName = _playerNamePlaceholder.text;
            }
            else
            {
                _tempPlayerName = _inputPlayerName.text;
            }

            _playerInfoSo.PlayerName = _tempPlayerName;
            _playerInfoSo.AvatarIndex = _selectedAvatarIndex;
            _playerInfoSo.RoomName = _tempRoomName;

            StartGame(GameMode.Shared, _tempRoomName, _gameScenePath);
        }

        private async void StartGame(GameMode mode, string roomName, string sceneName)
        {
            _runnerInstance = FindObjectOfType<NetworkRunner>();
            if (_runnerInstance == null)
            {
                _runnerInstance = Instantiate(_networkRunnerPrefab);
            }

            _buttonStart.interactable = false;
            _textConnectionStatus.text = "Connecting...";

            // Let the Fusion Runner know that we will be providing user input
            _runnerInstance.ProvideInput = true;
            await ShowLoadingScreen();
            var startGameArgs = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomName,
                Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(_gameScenePath)),
            };
            // GameMode.Host = Start a session with a specific name
            // GameMode.Client = Join a session with a specific name
            await _runnerInstance.StartGame(startGameArgs);

            if (_runnerInstance.IsServer)
            {
                await _runnerInstance.LoadScene(sceneName);
            }
        }

        private async UniTask ShowLoadingScreen()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
            _loadingCanvas.SetActive(true);
        }
    }
}
