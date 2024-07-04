using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
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


        public bool IsReady { get; private set; }

        public string UserName => _userName;

        private void Awake()
        {
            IsReady = false;
        }

        private void Start()
        {
            VivoxService.Instance.LoggedIn += OnUserLoggedIn;
            VivoxService.Instance.LoggedOut += OnUserLoggedOut;

            // Lobby
            VivoxService.Instance.ConnectionRecovered += OnConnectionRecovered;
            VivoxService.Instance.ConnectionRecovering += OnConnectionRecovering;
            VivoxService.Instance.ConnectionFailedToRecover += OnConnectionFailedToRecover;

#if !(UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID)
            IsReady = false;
            return;
#endif

            if ( _useDeviceAsName )
            {
                var deviceName = string.IsNullOrWhiteSpace(SystemInfo.deviceName) == false
                    ? SystemInfo.deviceName
                    : Environment.MachineName;
                _userName =
                    deviceName.Substring(0,
                        Math.Min(KDefaultMaxStringLength, deviceName.Length));
            }

            IsReady = true;
        }

        private void OnDestroy()
        {
            VivoxService.Instance.LoggedIn -= OnUserLoggedIn;
            VivoxService.Instance.LoggedOut -= OnUserLoggedOut;

            VivoxService.Instance.ConnectionRecovered -= OnConnectionRecovered;
            VivoxService.Instance.ConnectionRecovering -= OnConnectionRecovering;
            VivoxService.Instance.ConnectionFailedToRecover -= OnConnectionFailedToRecover;
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
            VivoxVoiceManager.Instance.SetRoomName($"Room{roomId}");
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
        }

        private Task JoinLobbyChannel()
        {
            Debug.Log($" *** JoinLobbyChannel {VivoxVoiceManager.Instance.RoomNameOrDefault}");
            return VivoxService.Instance.JoinGroupChannelAsync(VivoxVoiceManager.Instance.RoomNameOrDefault,
                ChatCapability.AudioOnly);
        }

        public async void JoinRoom()
        {
            await JoinLobbyChannel();
        }

        public async void LogoutOfVivoxServiceAsync()
        {
            await VivoxService.Instance.LogoutAsync();
            AuthenticationService.Instance.SignOut();
            if ( _vivoxChatUI != null ) _vivoxChatUI.SetActive(false);
        }

        private async Task LoginToVivox()
        {
            if ( string.IsNullOrWhiteSpace(_userName) ) return;
            var validName = Regex.Replace(_userName, "[^a-zA-Z0-9_-]", "");

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
            Debug.Log("______LoginToVivoxBool step 2");
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
            if ( _vivoxChatUI != null ) _vivoxChatUI.SetActive(false);
        }

        private void OnUserLoggedIn()
        {
            Debug.Log("*** LOGGED IN VIVOX****");
        }

        private void OnUserLoggedOut()
        {
            if ( _vivoxChatUI != null ) _vivoxChatUI.SetActive(false);
            Debug.Log("*** LOGGED OUT VIVOX****");
        }
    }
}