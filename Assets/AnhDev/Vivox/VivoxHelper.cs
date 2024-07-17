using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
#if AUTH_PACKAGE_PRESENT
using Unity.Services.Authentication;
#endif


namespace ConnectSphere
{
    public class VivoxHelper : MonoBehaviour
    {
        [SerializeField] private float _timeout = 3f;
        
        [Header("Credentials")]
        
        [SerializeField]
        private string _key;
        [SerializeField]
        private string _issuer;
        [SerializeField]
        private string _domain;
        [SerializeField]
        private string _server;
        
        [Header("Editor Test")] [SerializeField]
        private string _playerName;
        
        private async void Start()
        {
            await CheckVivoxService();

            RegisterVivoxServiceEvents();
            await InitializeUnityServices();
            await JoinVivox("hoanganh");
            await JoinChatRoom("hoanganhroom");
        }

        private async UniTask<bool> CheckVivoxService()
        {
            await InitializeUnityServices();
            var checkNull = UniTask.WaitUntil(() => VivoxService.Instance != null);
            var timeout = UniTask.WaitForSeconds(_timeout);
            await UniTask.WhenAny(checkNull, timeout);
            if ( VivoxService.Instance == null )
            {
                Debug.LogWarning("Cannot start Vivox service");
                return false;
            }

            return true;
        }

        [Button]
        public async UniTask JoinVivox(string playerName)
        {
            await InitializeVivox(playerName);
            await LoginVivox(playerName);
        }

        public async UniTask InitializeUnityServices()
        {
            var options = new InitializationOptions();
            
            if (CheckManualCredentials())
            {
                options.SetVivoxCredentials(_server, _domain, _issuer, _key);
            }
            await UnityServices.InitializeAsync(options);
            
            Debug.Log("*** UnityServices.InitializeAsync DONE");
        }
        
        public async UniTask InitializeVivox(string playerName)
        {

#if AUTH_PACKAGE_PRESENT
        if (!CheckManualCredentials())
        {
            Debug.Log("Vivox InitializeAsync step 2");
            AuthenticationService.Instance.SwitchProfile(playerName);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Vivox InitializeAsync step 3");
        }
#endif
            if ( !await CheckVivoxService() ) return;
            await VivoxService.Instance.InitializeAsync();
            Debug.Log($"Initialize Vivox done!");
        }
        
        public async UniTask LoginVivox(string playerName)
        {
            var loginOptions = new LoginOptions()
            {
                DisplayName = playerName,
                ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.OnePerSecond
            };

            await VivoxService.Instance.LoginAsync(loginOptions);
            Debug.Log($"Login Vivox done {VivoxService.Instance.SignedInPlayerId}");
            
            // foreach (var channel in VivoxService.Instance.ActiveChannels)
            // {
            //     Debug.Log($"{channel.Key}");
            //     foreach (var vvParticipant in channel.Value)
            //     {
            //         Debug.Log($"---{vvParticipant.DisplayName}");
            //     }
            // }
        }
        
        [Button]
        public async UniTask JoinChatRoom(string chatRoomId)
        {
            Debug.Log($" *** JoinChat {chatRoomId}");
            await VivoxService.Instance.JoinGroupChannelAsync(chatRoomId, ChatCapability.TextOnly);
        }
        
        [Button]
        public async UniTask JoinVoiceRoom(string voiceRoomId)
        {
            Debug.Log($" *** JoinVoice {voiceRoomId}");
            await VivoxService.Instance.JoinGroupChannelAsync(voiceRoomId, ChatCapability.AudioOnly);
        }

        [Button]
        public async UniTask LeaveRoom(string chatRoomId)
        {
            await VivoxService.Instance.LeaveChannelAsync(chatRoomId);
        }

        [Button]
        public async UniTask LogoutVivox()
        {
            await VivoxService.Instance.LogoutAsync();
        }
        
        private void RegisterVivoxServiceEvents()
        {
            if ( VivoxService.Instance != null )
            {
                VivoxService.Instance.LoggedIn += OnUserLoggedIn;
                VivoxService.Instance.LoggedOut += OnUserLoggedOut;

                VivoxService.Instance.ConnectionRecovered += OnConnectionRecovered;
                VivoxService.Instance.ConnectionRecovering += OnConnectionRecovering;
                VivoxService.Instance.ConnectionFailedToRecover += OnConnectionFailedToRecover;
            }
        }
        
        private void UnregisterVivoxServiceEvents()
        {
            if ( VivoxService.Instance != null )
            {
                VivoxService.Instance.LoggedIn -= OnUserLoggedIn;
                VivoxService.Instance.LoggedOut -= OnUserLoggedOut;

                VivoxService.Instance.ConnectionRecovered -= OnConnectionRecovered;
                VivoxService.Instance.ConnectionRecovering -= OnConnectionRecovering;
                VivoxService.Instance.ConnectionFailedToRecover -= OnConnectionFailedToRecover;
            }
        }
        
        private void OnDestroy()
        {
            UnregisterVivoxServiceEvents();
        }
        
        private bool CheckManualCredentials()
        {
            return !(string.IsNullOrEmpty(_issuer) && string.IsNullOrEmpty(_domain) && string.IsNullOrEmpty(_server));
        }
        
        
        private void OnConnectionRecovering()
        {
            Debug.Log("<color=green>\tConnectionRecovering</color>");
        }

        private void OnConnectionRecovered()
        {
            Debug.Log("<color=green>\tConnectionRecovered</color>");
        }

        private void OnConnectionFailedToRecover()
        {
            Debug.Log("<color=green>\tConnectionFailedToRecovered</color>");
        }

        private void OnUserLoggedIn()
        {
            Debug.Log("*** LOGGED IN VIVOX****");
        }

        private void OnUserLoggedOut()
        {
            Debug.Log("*** LOGGED OUT VIVOX****");
        }
    }
}

