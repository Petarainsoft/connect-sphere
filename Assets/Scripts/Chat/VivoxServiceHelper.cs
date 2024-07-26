using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AccountManagement;
using ConnectSphere;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Fusion;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

namespace Chat
{
    public class VivoxServiceHelper : MonoBehaviour
    {
        private const int KDefaultMaxStringLength = 15;

        private int mPermissionAskedCount;

        [SerializeField] private bool _useDeviceAsName = false;
        [SerializeField] private string _userName;
        [SerializeField] private GameObject _vivoxChatUI;

        [SerializeField] private GameManager _gameManager;
        

        [SerializeField] private Button _openChat;


        public bool IsReady { get; private set; }

        public string UserName => _userName;
        
        
        // REGION: ADAPT MULTIPLAYER
        [SerializeField]
        private TMP_Text gloablRoomName;
        [SerializeField]
        private TMP_Text gloablRoomNameInChat;
        
        [SerializeField]
        private TMP_Text gloablListChatName;

        [SerializeField]
        private UIToggle ChatToggle;


        [SerializeField] private GameObject _loadingScreen;


        [SerializeField] private PlayerInfoSO _playerInfoSo;
        
        [SerializeField] private TMP_Text _chatFrameTitle;

        public bool IsReadyForVoiceAndChat = false;

        [SerializeField] private TextChatUI _textChatUI;
        


        private void Awake()
        {
            IsReady = false;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(()=>VivoxService.Instance != null);
            VivoxService.Instance.LoggedIn += OnUserLoggedIn;
            VivoxService.Instance.LoggedOut += OnUserLoggedOut;

            // Lobby
            VivoxService.Instance.ConnectionRecovered += OnConnectionRecovered;
            VivoxService.Instance.ConnectionRecovering += OnConnectionRecovering;
            VivoxService.Instance.ConnectionFailedToRecover += OnConnectionFailedToRecover;
            
            _openChat.onClick.AddListener(() =>
            {
                _vivoxChatUI.SetActive(!_vivoxChatUI.activeSelf);
            });

            // _loadingScreen.SetActive(true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            
            // auto connect when running, comment this when running the VideoCall-Vivox scene
            Debug.Log($"<color=blue>CALLLING VIXOOOOV {_playerInfoSo.RoomName}</color>");
            // LoginVivoxAndJoinRoom(_playerInfoSo.RoomName);
            // MenuManager.Instance.RoomName
            gloablRoomName.text = $"{_playerInfoSo.RoomName} Office";
            gloablRoomNameInChat.text = $"{_playerInfoSo.RoomName} Office";
            gloablListChatName.text = $"{_playerInfoSo.RoomName} Office";
   
        }

        private bool isJoiningAudio = false;

        public async UniTask JoinAudio(int areaId)
        {
            if ( isJoiningAudio ) return;
            isJoiningAudio = true;
            if ( !IsMeJoinedAudio(areaId) )
            {
                AccountManagement.Utils.ShowLoading();
                await VivoxService.Instance.JoinGroupChannelAsync($"{_playerInfoSo.RoomName}_audio_{areaId}", ChatCapability.AudioOnly);
                AccountManagement.Utils.HideLoading();
            }

            isJoiningAudio = false;
        }

        public async UniTask LeaveAudio(int areaId)
        {
            if (IsMeJoinedAudio(areaId))
            {
                AccountManagement.Utils.ShowLoading();
                await VivoxService.Instance.LeaveChannelAsync($"{_playerInfoSo.RoomName}_audio_{areaId}");
                AccountManagement.Utils.HideLoading();
            }
        }

        private bool IsMeJoinedAudio(int areaId)
        {
            return VivoxService.Instance.ActiveChannels.ContainsKey($"{_playerInfoSo.RoomName}_audio_{areaId}") &&
                   VivoxService.Instance.ActiveChannels[$"{_playerInfoSo.RoomName}_audio_{areaId}"].Count > 0 &&
                   VivoxService.Instance.ActiveChannels[$"{_playerInfoSo.RoomName}_audio_{areaId}"].Any(p => p.IsSelf);
        }
        
        private bool IsMeJoinedChat(int areaId)
        {
            return VivoxService.Instance.ActiveChannels.ContainsKey($"{_playerInfoSo.RoomName}_chat_{areaId}") &&
                   VivoxService.Instance.ActiveChannels[$"{_playerInfoSo.RoomName}_chat_{areaId}"].Count > 0 &&
                   VivoxService.Instance.ActiveChannels[$"{_playerInfoSo.RoomName}_chat_{areaId}"].Any(p => p.IsSelf);
        }
        
        private bool IsMeJoinedGlobalChat()
        {
            return VivoxService.Instance.ActiveChannels.ContainsKey($"chat_{_playerInfoSo.RoomName}") &&
                   VivoxService.Instance.ActiveChannels[$"chat_{_playerInfoSo.RoomName}"].Count > 0 &&
                   VivoxService.Instance.ActiveChannels[$"chat_{_playerInfoSo.RoomName}"].Any(p => p.IsSelf);
        }

        public async void JoinGlobalChat()
        {
            _chatFrameTitle.text = $"{_playerInfoSo.RoomName} Office";
            if ( !IsMeJoinedGlobalChat() )
            {
                AccountManagement.Utils.ShowLoading();
                await VivoxService.Instance.JoinGroupChannelAsync($"chat_{_playerInfoSo.RoomName}",
                    ChatCapability.TextOnly);
                AccountManagement.Utils.HideLoading();
            } 
            else
            {
                _textChatUI.OnChannelJoined($"chat_{_playerInfoSo.RoomName}");
            }
        }
        
        public async void LeaveGlobalChat()
        {
            if ( IsMeJoinedGlobalChat() )
            {
                AccountManagement.Utils.ShowLoading();
                await VivoxService.Instance.LeaveChannelAsync($"chat_{_playerInfoSo.RoomName}");
                AccountManagement.Utils.HideLoading();
            }
            else
            {
                _textChatUI.ResetChannelName();
            }
        }
        
        public async UniTask JoinAreaChat(int areaId)
        {
            if ( !IsMeJoinedChat(areaId) )
            {
                AccountManagement.Utils.ShowLoading();
                await VivoxService.Instance.JoinGroupChannelAsync($"{_playerInfoSo.RoomName}_chat_{areaId}", ChatCapability.TextOnly);
                AccountManagement.Utils.HideLoading();
            }
            else
            {
                _textChatUI.OnChannelJoined($"{_playerInfoSo.RoomName}_chat_{areaId}");
            }
        }
        
        public async UniTask LeaveAreaChat(int areaId)
        {
            if ( IsMeJoinedChat(areaId) )
            {
                AccountManagement.Utils.ShowLoading();
                VivoxService.Instance.LeaveChannelAsync($"{_playerInfoSo.RoomName}_chat_{areaId}");
                _textChatUI.ResetChannelName();
                AccountManagement.Utils.HideLoading();
            }
            else
            {
                _textChatUI.ResetChannelName();
            }
        }

        private PermissionHelper _microphoneHelper = new PermissionHelper(Permission.Microphone);
        
        private void AskPermissionAudio()
        {
            _microphoneHelper.OnPermissionResult = AfterRequestPermission;
            _microphoneHelper.Ask();
        }
        
        private void AfterRequestPermission(bool requestOk)
        {
            Debug.Log($"Request Permission with {requestOk}");
            if ( requestOk )
            {
                if ( VivoxService.Instance != null ) VivoxService.Instance.UnmuteInputDevice();
            }
        }

        public void ToggleAudio(bool isOn)
        {
            if ( !isOn )
            {
                AskPermissionAudio();
            }
            else
            {
                if ( VivoxService.Instance != null ) VivoxService.Instance.MuteInputDevice();
            }
        }

        public async void SignOut()
        {
            if ( VivoxService.Instance != null )
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
                await VivoxService.Instance.LogoutAsync();
            }

            if ( ApiManager.Instance != null ) ApiManager.Instance.Logout();
            if ( AuthenticationService.Instance != null ) AuthenticationService.Instance.SignOut();
            _gameManager.Shutdown();
        }
        
        private void Update()
        {

#if !(UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID)
            IsReady = false;
            return;
#endif
            if ( IsReady ) return;
            #if UNITY_EDITOR
            // var callerId = ParrelSync.ClonesManager.GetArgument();
            // if (string.IsNullOrWhiteSpace(callerId)) callerId = "0";
            // _userName = $"Caller {callerId}";
            _userName = _playerInfoSo.PlayerName;
            #else

            if ( _useDeviceAsName )
            {
                var deviceName = string.IsNullOrWhiteSpace(SystemInfo.deviceName) == false
                    ? SystemInfo.deviceName
                    : Environment.MachineName;
                _userName =
                    deviceName.Substring(0,
                        Math.Min(KDefaultMaxStringLength, deviceName.Length));
            }
            else {
               _userName = _playerInfoSo.PlayerName;
            }
            #endif

            IsReady = true;
        }

        private void OnDestroy()
        {
            VivoxService.Instance.LoggedIn -= OnUserLoggedIn;
            VivoxService.Instance.LoggedOut -= OnUserLoggedOut;

            VivoxService.Instance.ConnectionRecovered -= OnConnectionRecovered;
            VivoxService.Instance.ConnectionRecovering -= OnConnectionRecovering;
            VivoxService.Instance.ConnectionFailedToRecover -= OnConnectionFailedToRecover;
            
            _openChat.onClick.RemoveAllListeners();
        }

#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
    bool IsAndroid12AndUp()
    {
        // android12VersionCode is hardcoded because it might not be available in all versions of Android SDK
        const int android12VersionCode = 31;
        AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION");
        int buildSdkVersion = buildVersionClass.GetStatic<int>("SDK_INT");

        return buildSdkVersion >= android12VersionCode;
    }

    string GetBluetoothConnectPermissionCode()
    {
        if (IsAndroid12AndUp())
        {
            // UnityEngine.Android.Permission does not contain the BLUETOOTH_CONNECT permission, fetch it from Android
            AndroidJavaClass manifestPermissionClass = new AndroidJavaClass("android.Manifest$permission");
            string permissionCode = manifestPermissionClass.GetStatic<string>("BLUETOOTH_CONNECT");

            return permissionCode;
        }

        return "";
    }
#endif

        bool IsMicPermissionGranted()
        {
            bool isGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (IsAndroid12AndUp())
        {
            // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission for all features to work
            isGranted &= Permission.HasUserAuthorizedPermission(GetBluetoothConnectPermissionCode());
        }
#endif
            return isGranted;
        }

        void AskForPermissions()
        {
            string permissionCode = Permission.Microphone;

#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (mPermissionAskedCount == 1 && IsAndroid12AndUp())
        {
            permissionCode = GetBluetoothConnectPermissionCode();
        }
#endif
            mPermissionAskedCount++;
            Permission.RequestUserPermission(permissionCode);
        }

        bool IsPermissionsDenied()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission
        if (IsAndroid12AndUp())
        {
            return mPermissionAskedCount == 2;
        }
#endif
            return mPermissionAskedCount == 1;
        }

        public void LoginToVivoxService()
        {
            if ( IsMicPermissionGranted() )
            {
                // The user authorized use of the microphone.
                LoginToVivox();
            }
            else
            {
                // We do not have the needed permissions.
                // Ask for permissions or proceed without the functionality enabled if they were denied by the user
                if ( IsPermissionsDenied() )
                {
                    mPermissionAskedCount = 0;
                    LoginToVivox();
                }
                else
                {
                    AskForPermissions();
                }
            }
        }

        public async Task LoginVivoxAndJoinRoom(string roomId)
        {
            VivoxVoiceManager.Instance.SetRoomName(roomId);
            Debug.Log("LoginVivoxAndJoinRoom step 1");
            var loggedIn = false;
            if ( IsMicPermissionGranted() )
            {
                Debug.Log("LoginVivoxAndJoinRoom step 2");
                // The user authorized use of the microphone.
                loggedIn = await LoginToVivoxBool();
            }
            else
            {
                // We do not have the needed permissions.
                // Ask for permissions or proceed without the functionality enabled if they were denied by the user
                if ( IsPermissionsDenied() )
                {
                    Debug.Log("LoginVivoxAndJoinRoom step 3");
                    mPermissionAskedCount = 0;
                    loggedIn = await LoginToVivoxBool();
                }
                else
                {
                    Debug.Log("LoginVivoxAndJoinRoom step 4");
                    AskForPermissions();
                }
            }

            if ( !loggedIn ) return;

            Debug.Log("LoginVivoxAndJoinRoom step 5");
            try
            {
                await JoinLobbyChannel();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            Debug.Log("LoginVivoxAndJoinRoom step 6");
            if ( _vivoxChatUI != null ) _vivoxChatUI.SetActive(true);
            ChatToggle.ToggleOn();
            ChatToggle.ExecuteClick();
            _loadingScreen.SetActive(false);
            IsReadyForVoiceAndChat = true;
        }

        private Task JoinLobbyChannel()
        {
            Debug.Log($" *** JoinLobbyChannel {VivoxVoiceManager.Instance.RoomNameOrDefault}");
            return VivoxService.Instance.JoinGroupChannelAsync(VivoxVoiceManager.Instance.RoomNameOrDefault,
                ChatCapability.TextAndAudio);
        }

        public async void JoinRoom()
        {
            await JoinLobbyChannel();
        }

        public async void LogoutOfVivoxServiceAsync()
        {
            await VivoxService.Instance.LeaveChannelAsync(VivoxVoiceManager.Instance.RoomNameOrDefault);
            await VivoxService.Instance.LogoutAsync();
            AuthenticationService.Instance.SignOut();
            // if ( _vivoxChatUI != null ) _vivoxChatUI.SetActive(false);
            IsReadyForVoiceAndChat = false;
        }

        private async Task LoginToVivox()
        {
            if ( string.IsNullOrWhiteSpace(_userName) ) return;
            var validName = Regex.Replace(_userName, "[^a-zA-Z0-9_-]", "");

            Debug.Log($"<color=green>\tLogin VIVVOX With {_userName}</color>");
            await VivoxVoiceManager.Instance.InitializeAsync(validName);
            var loginOptions = new LoginOptions()
            {
                DisplayName = validName,
                ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.OnePerSecond
            };

            await VivoxService.Instance.LoginAsync(loginOptions);
        }

        private async Task<bool> LoginToVivoxBool()
        {
            Debug.Log("______LoginToVivoxBool step 1");
            if ( string.IsNullOrWhiteSpace(_userName) ) return false;
            _userName = _userName.Replace(" ", string.Empty);
            Debug.Log("______LoginToVivoxBool step 2");
            Debug.Log($"<color=green>\tLogin VIVVOX With {_userName}</color>");
            var validName = Regex.Replace(_userName, "[^a-zA-Z0-9_-]", "");

            await VivoxVoiceManager.Instance.InitializeAsync(validName);
            Debug.Log("______LoginToVivoxBool step 3");
            var loginOptions = new LoginOptions()
            {
                DisplayName = validName,
                ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.OnePerSecond
            };

            await VivoxService.Instance.LoginAsync(loginOptions);
            Debug.Log("______LoginToVivoxBool step 4");
            return true;
        }


        void OnConnectionRecovering()
        {
            Debug.Log("<color=green>\tConnectionRecovering</color>");
        }

        void OnConnectionRecovered()
        {
            Debug.Log("<color=green>\tConnectionRecovered</color>");
        }

        void OnConnectionFailedToRecover()
        {
            Debug.Log("<color=green>\tConnectionFailedToRecovered</color>");
            // if ( _vivoxChatUI != null ) _vivoxChatUI.SetActive(false);
            IsReadyForVoiceAndChat = false;
        }

        private void OnUserLoggedIn()
        {
            Debug.Log("*** LOGGED IN VIVOX****");
        }

        private void OnUserLoggedOut()
        {
            if ( _vivoxChatUI != null ) _vivoxChatUI.SetActive(false);
            Debug.Log("*** LOGGED OUT VIVOX****");
            IsReadyForVoiceAndChat = false;
        }
    }
}