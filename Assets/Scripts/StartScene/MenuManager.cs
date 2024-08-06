using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using System.Reflection;

namespace ConnectSphere
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] private NetworkRunner _networkRunnerPrefab;

        [Header("Network Canvas")] [SerializeField]
        private TMP_InputField _inputRoomName;

        [SerializeField] private TextMeshProUGUI _roomNamePlaceholder;

        [Header("Selection Canvas")] [SerializeField]
        private TMP_InputField _inputPlayerName;

        [SerializeField] private TextMeshProUGUI _playerNamePlaceholder;
        [SerializeField] private Transform _avatarsContainer;
        [SerializeField] private TextMeshProUGUI _textConnectionStatus;
        [SerializeField] private Button _buttonStart;

        [Header("Others")] [SerializeField] GameObject _loadingCanvas;
        [SerializeField] GameObject _networkCanvasObject;
        [SerializeField] GameObject _selectionCanvasObject;
        [SerializeField] private PlayerInfoSO _playerInfoSo;
        [SerializeField] private string _gameScenePath;


        [Header("Vivox")] [SerializeField] private float _timeout = 3f;


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
            if ( string.IsNullOrEmpty(_inputRoomName.text.Trim()) )
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

        public void OnBackToNetworkClicked()
        {
            _tempRoomName = string.Empty;
            _networkCanvasObject.SetActive(true);
            _selectionCanvasObject.SetActive(false);
            OnAvatarImageClicked?.Invoke(0);
        }

        private void HandleSelectedAvatar(int index)
        {
            foreach (Transform child in _avatarsContainer)
            {
                if ( child.GetSiblingIndex() == index )
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
            if ( string.IsNullOrEmpty(_inputPlayerName.text.Trim()) )
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
            _playerInfoSo.Email = PlayerPrefs.GetString("username");
            _playerInfoSo.DatabaseId = PlayerPrefs.GetInt("userId");

            StartGame(GameMode.Shared, _tempRoomName, _gameScenePath);
        }

        private async UniTask<bool> JoinVivox(string playerEmail)
        {
            Debug.Log("** Initialize Unity Service");
            var initializationOptions = new InitializationOptions();
            
            initializationOptions.SetVivoxCredentials(
                server: "https://unity.vivox.com/appconfig/10793-conne-77095-udash",
                domain: "mtu1xp.vivox.com",
                issuer: "10793-conne-77095-udash",
                "8OZBvVqIzQMq1qqMQ3C23DWrrXNJrVuM");
            
            await UnityServices.InitializeAsync( initializationOptions);
            var validName = playerEmail.Replace("@","_").Replace(".","_");
            AuthenticationService.Instance.SwitchProfile(validName);
            Debug.Log("** Sign In AuthenticationService");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("** Sign In AuthenticationService 2");
            var checkNull = UniTask.WaitUntil(() => VivoxService.Instance != null);
            Debug.Log("** Sign In AuthenticationService 3");
            var timeout = UniTask.WaitForSeconds(_timeout);
            Debug.Log("** Sign In AuthenticationService 4");
            await UniTask.WhenAny(checkNull, timeout);
            if ( VivoxService.Instance == null )
            {
                Debug.Log("** Sign In AuthenticationService 5");
                Debug.LogWarning("** Cannot start Vivox service");
                return false;
            }
            Debug.Log("** Sign In AuthenticationService 6");
            await VivoxService.Instance.InitializeAsync();
            Debug.Log($"** Initialize Vivox done!");
            Debug.Log("** Sign In AuthenticationService 7");

            var loginOptions = new LoginOptions()
            {
                DisplayName = validName,
                ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.FivePerSecond
            };
            Debug.Log("** Sign In AuthenticationService 8");
            await VivoxService.Instance.LoginAsync(loginOptions);
            Debug.Log("** Sign In AuthenticationService 9");
            Debug.Log($"** Login vivox done!");
            return true;
        }

        private async void StartGame(GameMode mode, string roomName, string sceneName)
        {
            _runnerInstance = FindObjectOfType<NetworkRunner>();
            if ( _runnerInstance == null )
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

            try
            {
                if ( !await JoinVivox(_playerInfoSo.Email.Trim()) )
                {
                    var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                    warningPopup.Data.SetButtonsLabels("Ok");
                    warningPopup.Data.SetLabelsTexts("Chat Service", "Currently Chat feature isn't available!");
                    warningPopup.Show();
                    await UniTask.WaitUntil(() => warningPopup.IsDestroyed());
                }
            }
            catch (Exception e)
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Services Error",
                    "Currently voice/chat isn't available!\nRetry again!");
                warningPopup.Show();
                await UniTask.WaitUntil(() => warningPopup.IsDestroyed());
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // GameMode.Host = Start a session with a specific name
            // GameMode.Client = Join a session with a specific name
            await _runnerInstance.StartGame(startGameArgs);

            if ( _runnerInstance.IsServer )
            {
                await _runnerInstance.LoadScene(sceneName);
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private async UniTask ShowLoadingScreen()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
            _loadingCanvas.SetActive(true);
        }
    }
}