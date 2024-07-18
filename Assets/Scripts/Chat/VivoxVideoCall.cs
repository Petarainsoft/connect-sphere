using System;
using System.Collections;
using System.Collections.Generic;
using AccountManagement;
using Chat.States;
using ConnectSphere;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using ParrelSync;
#endif
using TMPro;
using Unity.RenderStreaming;
using Unity.Services.Vivox;
using Unity.VisualScripting;
using VInspector;

namespace Chat
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    using Fusion;

    /// <summary>
    /// Handle both video streaming using webrtc and audio using vivox
    /// </summary>
    [RequireComponent(typeof(VivoxVideoCallFsm))]
    public class VivoxVideoCall : MonoBehaviour
    {
        [SerializeField] private List<SignalingManager> _renderStreamings;
        [SerializeField] private TMP_Dropdown _webcamSelectDropdown;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _setUpButton;
        [SerializeField] private Button _hangUpButton;
        [SerializeField] private InputField _connectionIdInput;
        [SerializeField] private RawImage _localVideoImage;
        [SerializeField] private RawImage _trayBarVideoImage;
        [SerializeField] private List<RawImage> _remoteVideoImages;

        [SerializeField] private GameObject _videoSinglePrefab;

        [SerializeField] private List<VideoSingleConnection> _availableConnection = new List<VideoSingleConnection>();


        // anhnguyen tempo comment
        [SerializeField] private VivoxServiceHelper _vivoxHelper;

        [SerializeField] private TMP_Text _screenText;
        [SerializeField] private InputField _callIndexInput;


        [SerializeField] private int callIndex = 0;


        private string connectionId;

        [SerializeField] private RenderStreamingSettings _settings;

        [SerializeField] private PlayerInfoSO _playerSO;

        [Header("Gather Areas Handle Connection")] [SerializeField]
        private List<GatheringArea> _areas;
        

        private VivoxVideoCallFsm fsm;

        private WebCamTexture webcamTexture;
        private const float WaitTime = 1f;

        private async UniTask<bool> RequestCamera()
        {

            if ( _localVideoImage == null || _trayBarVideoImage == null)
            {
                Debug.LogError("Texture for showing camera is null");
                return false;
            }
            
            var timeout = UniTask.WaitForSeconds(4f);
            var waitForCameraAvailable =
                UniTask.WaitUntil(() => WebCamTexture.devices != null && WebCamTexture.devices.Length >= 1);
            await UniTask.WhenAny(timeout, waitForCameraAvailable);

            if ( WebCamTexture.devices == null || WebCamTexture.devices.Length == 0 )
            {
                return false;
            }
            
            
            var cameraDevice = WebCamTexture.devices[0];
            if (WebCamTexture.devices.Length > 1) cameraDevice = WebCamTexture.devices[1]; // front camera for mobile devices
            
            webcamTexture = new WebCamTexture(cameraDevice.name, 600, 480, (int)20);
            webcamTexture.Play();
          
            await UniTask.WaitUntil(() => webcamTexture.didUpdateThisFrame);
            Debug.Log($"CameraTexture Size w:{webcamTexture.width} h{webcamTexture.height}");
        
            
            _localVideoImage.texture = webcamTexture;
            _trayBarVideoImage.texture = webcamTexture;
            
            Debug.Log($"Container Size w:{_localVideoImage.texture.width} h{_localVideoImage.texture.height}");
            
            for (var i = 0; i < _availableConnection.Count; i++)
            {
                var connection = _availableConnection[i];
                Debug.Log("1");
                Debug.Log($"WebcamTexture is null {_localVideoImage.texture == null}");
                connection.webCamStreamer.sourceTexture = _localVideoImage.texture;
                Debug.Log("2");
                connection.webCamStreamer.OnStartedStream += id => connection.receiveVideoViewer.enabled = true;
                // connection.webCamStreamer.OnStartedStream += _ =>
                // {
                //     #if !UNITY_EDITOR
                //     _localVideoImage.texture = mWebcamTexture;
                //     #endif
                // };
            
                if ( _settings != null )
                {
                    connection.webCamStreamer.width = (uint)_settings.StreamSize.x;
                    connection.webCamStreamer.height = (uint)_settings.StreamSize.y;
            
                    _settings.ApplyH264Codec();
                }
            
                connection.SetTextureIndex(i);
                connection.SetTextureReceiveCb(OnTextureReceive);
            }
            
            await UniTask.WaitForEndOfFrame(this);
            await UniTask.WaitForSeconds(1f);
            
            // anhnguyen, for demo, run right after having webcam working
            // yield return new WaitUntil(() => Runner != null && Runner.ActivePlayers != null);
            Debug.Log($"<color=yellow>Start calling {_playerSO.RoomName} ___ indexCall {0}</color>");
            await UniTask.WaitUntil(() => _vivoxHelper.IsReadyForVoiceAndChat);
            // SetUp(_playerSO.RoomName, 1);

            return true;
        }

        [Button]
        public void DoWebRTC()
        {
            // SetUp(_playerSO.RoomName, 2);
        }
        
        private Texture2D RotateTexture90Degrees(WebCamTexture originalTexture)
        {
            int width = originalTexture.width;
            int height = originalTexture.height;
            Texture2D rotatedTexture = new Texture2D(height, width);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    rotatedTexture.SetPixel(height - y - 1, x, originalTexture.GetPixel(x, y));
                }
            }

            rotatedTexture.Apply();
            return rotatedTexture;
        }


        private void Awake()
        {
            fsm = GetComponent<VivoxVideoCallFsm>();
#if UNITY_EDITOR
            if ( ClonesManager.IsClone() )
            {
                callIndex = int.Parse(ClonesManager.GetArgument());
            }
            else
            {
                callIndex = 0;
            }
#else
            // callIndex = int.Parse(_callIndexInput.text.Trim());
#endif
            _screenText.text = $"Screen {callIndex}";


            _startButton.interactable = true;
            _webcamSelectDropdown.interactable = true;
            _setUpButton.interactable = false;
            _hangUpButton.interactable = false;
            _connectionIdInput.interactable = true;

            // _setUpButton.onClick.AddListener(SetUp);
            _hangUpButton.onClick.AddListener(HangUp);
            _connectionIdInput.onValueChanged.AddListener(input => connectionId = input);
            _connectionIdInput.text = $"{Random.Range(0, 99999):D5}";
            _webcamSelectDropdown.onValueChanged.AddListener(index =>
                _availableConnection.First().webCamStreamer.sourceDeviceIndex = index);
            _webcamSelectDropdown.options = WebCamTexture.devices.Select(x => x.name)
                .Select(x => new TMP_Dropdown.OptionData(x)).ToList();

            GatheringArea.OnPlayerEnteredArea += HandlePlayerEnter;
            GatheringArea.OnPlayerExitArea += HandlePlayerExit;
        }
        
        private void OnDestroy()
        {
            GatheringArea.OnPlayerEnteredArea -= HandlePlayerEnter;
            GatheringArea.OnPlayerExitArea -= HandlePlayerExit;
        }

        private void HandlePlayerExit(int areaId)
        {
            Debug.Log($"<color=red>PlayerEXIT {areaId}</color>");
            var area = _areas.FirstOrDefault(e => e.AreaId == areaId);
            if ( area == null ) return;
            var listPlayers = area.PlayersInArea;
            
            // me went out
            var myId = PlayerPrefs.GetInt("userId");
            if ( !listPlayers.Any(p => p.GetComponent<Player>().DatabaseId == myId) )
            {
                var listOtherConnection = new List<string>();
                foreach (var no in listPlayers)
                {
                    var id = no.GetComponent<Player>().DatabaseId;
                    if (id == myId) continue;
                    connectionId = areaId.ToString();
                    var connectionID = MakeConnectionUniqueId(id);
                    listOtherConnection.Add(connectionID);
                }
                
                var _availableConnectionIndex = -1;
                for (int i = 0; i <= _availableConnection.Count; i++)
                {

                    var con = _availableConnection[++_availableConnectionIndex];
                    if ( con.IsWorking )
                    {
                        foreach (var existingConnection in listOtherConnection)
                        {
                            if ( con.singleConnection != null && con.singleConnection.ExistConnection(existingConnection))
                            {
                                con.singleConnection.DeleteConnection(existingConnection);
                                con.IsWorking = false;
                                con.Release();
                                continue;
                            }
                        }
                    }
                }
            }
        }


        private async void HandlePlayerEnter(int areaId)
        {
            Debug.Log($"<color=red>PlayerENTER {areaId}</color>");
            var area = _areas.FirstOrDefault(e => e.AreaId == areaId);
            if ( area == null ) return;
            var listPlayers = area.PlayersInArea;
            
            // me went in
            var myId = PlayerPrefs.GetInt("userId");
            if ( listPlayers.Any(p => p.GetComponent<Player>().DatabaseId == myId) )
            {
                var listOtherConnection = new List<string>();
                foreach (var no in listPlayers)
                {
                    var id = no.GetComponent<Player>().DatabaseId;
                    if (id == myId) continue;
                    connectionId = areaId.ToString();
                   
                    var connectionID = MakeConnectionUniqueId(id);
                    listOtherConnection.Add(connectionID);
                }
                
                var _availableConnectionIndex = -1;
                for (int i = 0; i <= _availableConnection.Count; i++)
                {
                    var con = _availableConnection[++_availableConnectionIndex];
                    if ( !con.IsWorking )
                    {
                        foreach (var existingConnection in listOtherConnection)
                        {
                            if ( con.singleConnection != null && !con.singleConnection.ExistConnection(existingConnection))
                            {
                                con.singleConnection.CreateConnection(existingConnection);
                                con.IsWorking = true;
                                await UniTask.WaitUntil(() =>
                                    con.IsWorking && con.singleConnection.IsStable(existingConnection));
                                await UniTask.WaitForSeconds(WaitTime);
                            }
                        }
                    }
                }
            }
            
        }

        private bool callVivox = false;

        public void OnTextureReceive(Texture receiveTexture, int textureIndex)
        {
            Debug.Log($"Receive Texture for index {textureIndex}");
            _remoteVideoImages[textureIndex].texture = receiveTexture;
            _remoteVideoImages[textureIndex].transform.parent.gameObject.SetActive(true);

            // if ( textureIndex == 0 )
            // {
            Debug.Log("Resize local camera texture to top right");
            _localVideoImage.GetComponent<ToggleFullscreenUI>()?.SetFullScreen(false);
            _trayBarVideoImage.texture = _localVideoImage.texture;

            if ( !callVivox )
            {
                // Debug.Log($"Start ConnectingVivox to {connectionId}");
                Debug.Log($"<color=yellow>Start ConnectingVivox to {connectionId}</color>");
                // _vivoxHelper.LoginVivoxAndJoinRoom(connectionId);
                callVivox = true;
            }
            // }
        }

        public void StartMyCamera()
        {
            _startButton.interactable = false;
            _webcamSelectDropdown.interactable = false;
            _setUpButton.interactable = true;
            RequestCamera();
        }

        private IEnumerator Start()
        {
            _setUpButton.interactable = false;
            _hangUpButton.interactable = false;
            _connectionIdInput.interactable = false;

            yield return new WaitUntil(() => VivoxVoiceManager.Instance != null && VivoxVoiceManager.Instance.IsReady);
            Debug.Log("VivoxVoiceManager is ready");
            // yield return new WaitUntil(() => _vivoxHelper != null && _vivoxHelper.IsReady);
            Debug.Log("VivoxServiceHelper is ready");

            foreach (var _renderStreaming in _renderStreamings)
            {
                if ( _renderStreaming.runOnAwake )
                    yield break;
                if ( _settings != null )
                    _renderStreaming.useDefaultSettings = _settings.UseDefaultSettings;
                if ( _settings?.SignalingSettings != null )
                    _renderStreaming.SetSignalingSettings(_settings.SignalingSettings);
                _renderStreaming.Run();
            }

            Debug.Log("RenderingStreaming is running");
            _setUpButton.interactable = true;
            _hangUpButton.interactable = true;
            _connectionIdInput.interactable = true;
            
            
            
        }
        
        
        // private async void SetUp(string roomName, int inCallIndex)
        // {
        //     _setUpButton.interactable = false;
        //     _hangUpButton.interactable = true;
        //     _connectionIdInput.interactable = false;
        //     callIndex = inCallIndex;
        //     connectionId = roomName;
        //
        //     var _availableConnectionIndex = -1;
        //
        //     for (int i = 0; i <= _availableConnection.Count; i++)
        //     {
        //         if ( i == callIndex ) continue; //ignore self
        //
        //         var con = _availableConnection[++_availableConnectionIndex];
        //
        //         con.receiveVideoViewer.SetCodec(_settings.ReceiverVideoCodec);
        //         con.webCamStreamer.SetCodec(_settings.SenderVideoCodec);
        //
        //         var connectionUniqueId = MakeConnectionUniqueId(i);
        //
        //         if ( !con.IsWorking )
        //         {
        //             Debug.Log($"Start Connecting for {connectionUniqueId}");
        //             con.singleConnection.CreateConnection(connectionUniqueId);
        //             con.IsWorking = true;
        //
        //             await UniTask.WaitUntil(() =>
        //                 con.IsWorking && con.singleConnection.IsStable(connectionUniqueId));
        //
        //             // Debug.Log($"Connected for {connectionUniqueId}");
        //             await UniTask.WaitForSeconds(WaitTime);
        //         }
        //     }
        // }
        //
        //
        // private async void SetUp()
        // {
        //     _setUpButton.interactable = false;
        //     _hangUpButton.interactable = true;
        //     _connectionIdInput.interactable = false;
        //     callIndex = int.Parse(_callIndexInput.text.Trim());
        //
        //     var _availableConnectionIndex = -1;
        //
        //     for (int i = 0; i <= _availableConnection.Count; i++)
        //     {
        //         if ( i == callIndex ) continue; //ignore self
        //
        //         var con = _availableConnection[++_availableConnectionIndex];
        //
        //         con.receiveVideoViewer.SetCodec(_settings.ReceiverVideoCodec);
        //         con.webCamStreamer.SetCodec(_settings.SenderVideoCodec);
        //
        //         var connectionUniqueId = MakeConnectionUniqueId(i);
        //
        //         if ( !con.IsWorking )
        //         {
        //             Debug.Log($"Start Connecting for {connectionUniqueId}");
        //             con.singleConnection.CreateConnection(connectionUniqueId);
        //             con.IsWorking = true;
        //
        //             await UniTask.WaitUntil(() =>
        //                 con.IsWorking && con.singleConnection.IsStable(connectionUniqueId));
        //
        //             // Debug.Log($"Connected for {connectionUniqueId}");
        //             await UniTask.WaitForSeconds(WaitTime);
        //         }
        //     }
        // }

        private string MakeConnectionUniqueId(int i)
        {
            var connectionUniqueId = $"{connectionId}_{callIndex}_{i}";
            if ( callIndex > i ) connectionUniqueId = $"{connectionId}_{i}_{callIndex}";
            return connectionUniqueId;
        }


        private async void HangUp()
        {
            try
            {
                // singleConnection.DeleteConnection(connectionId);
                for (int i = 0; i < _availableConnection.Count; i++)
                {
                    if ( i == callIndex ) continue; //ignore self
                    var connectionUniqueId = MakeConnectionUniqueId(i);
                    var con = _availableConnection[i];
                    if ( con.IsWorking )
                    {
                        if ( con.singleConnection != null && con.singleConnection.ExistConnection(connectionUniqueId))
                        {
                            con.singleConnection.DeleteConnection(connectionUniqueId);
                        }
                    }
                    con.IsWorking = false;
                    con.Release();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Hangup Got Exception");
                Debug.LogException(e);
            }

            _setUpButton.interactable = true;
            _hangUpButton.interactable = false;
            _connectionIdInput.interactable = true;
            callVivox = false;
            _connectionIdInput.text = $"{Random.Range(0, 99999):D5}";
#if !UNITY_EDITOR
            _localVideoImage.texture = null;
#endif
            Debug.Log("Resize local camera texture to be full screen");
            _localVideoImage.GetComponent<ToggleFullscreenUI>()?.SetFullScreen(true);
            // _vivoxHelper.LogoutOfVivoxServiceAsync();
        }
    }
}