using System.Collections.Generic;
using AhnLab.EventSystem;
using ConnectSphere;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Unity.RenderStreaming;
using UnityEngine.Android;

namespace Chat
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Handle both video streaming using webrtc and audio using vivox
    /// </summary>
    public class VideoCallManager : MonoBehaviour
    {
        [SerializeField] private SignalingManager _signalingManager;
        [SerializeField] private RawImage _trayBarVideoImage;


        [SerializeField] private VideoCallListMonitor _callListMonitor;
        [SerializeField] private VideoConnectionPool _callPool;

        private readonly Dictionary<OrderedPeersInfo, VideoSingleConnection> currentCalls =
            new Dictionary<OrderedPeersInfo, VideoSingleConnection>();

        [SerializeField] private RenderStreamingSettings _settings;

        [SerializeField] private PlayerInfoSO _playerSO;

        [Header("Camera Requested Size")]
        [SerializeField] private int _requestedWidth = 640;
        [SerializeField] private int _requestedHeight = 480;
        [SerializeField] private int _requestedFps = 20;

        [SerializeField] private float _requestCameraTimeout = 5f;
        
        [SerializeField] private Texture2D _black;


        private WebCamTexture currentWebcamTexture;
        
        private readonly PermissionHelper cameraPermissionHelper = new PermissionHelper(Permission.Camera);

        private void Awake()
        {
            if ( _trayBarVideoImage == null )
            {
                Debug.LogError("Texture for showing camera is null");
            }
        }

        private async void Start()
        {
            if ( _signalingManager.runOnAwake ) return;
            if ( _settings != null && _settings?.SignalingSettings != null )
            {
                _signalingManager.useDefaultSettings = _settings.UseDefaultSettings;
                _signalingManager.SetSignalingSettings(_settings.SignalingSettings);
                _signalingManager.Configure();
                Debug.Log("RenderingStreaming is running");
            }

            await UniTask.DelayFrame(1);
            AskForCameraPermission();
        }

        private void AskForCameraPermission()
        {
            if ( cameraPermissionHelper == null )
            {
                Debug.LogError("cameraPermisisonHelper is not assigned!");
                return;
            }
            cameraPermissionHelper.OnPermissionResult = AfterRequestPermission;
            cameraPermissionHelper.Ask();
        }

        private void AfterRequestPermission(bool requestOk)
        {
            Debug.Log($"Request Permission with {requestOk}");
            if ( requestOk ) TryStartCamera();
            else
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Camera", "Camera Permission is denied!\nPlease grant permission from OS settings");
                warningPopup.HideOnClickOverlay = false;
                warningPopup.HideOnClickAnywhere = false;
                warningPopup.HideOnClickContainer = false;
                warningPopup.Data.SetButtonsCallbacks(()=>
                {
                    warningPopup.Hide();
                });
                warningPopup.Show();
            }
        }

        public void ToggleCamera(bool isOff)
        {
            if ( !isOff )
            {
                AskForCameraPermission();
            }
            else
            {
                if ( currentWebcamTexture != null ) currentWebcamTexture.Stop();
                if ( _trayBarVideoImage != null ) _trayBarVideoImage.texture = _black;
            }
        }

        private async UniTask<bool> TryStartCamera()
        {
            // wait until camera is available
            var getWebcam = UniTask.WaitUntil(() => WebCamTexture.devices != null && WebCamTexture.devices.Length >= 1);
            var timeout = UniTask.WaitForSeconds(_requestCameraTimeout);
            await UniTask.WhenAny(getWebcam, timeout);

            if ( WebCamTexture.devices == null || WebCamTexture.devices.Length == 0 )
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Camera", "Failed to start camera");
                warningPopup.HideOnClickOverlay = false;
                warningPopup.HideOnClickAnywhere = false;
                warningPopup.HideOnClickContainer = false;
                warningPopup.Data.SetButtonsCallbacks(()=>
                {
                    warningPopup.Hide();
                });
                warningPopup.Show();
                await UniTask.WaitUntil(warningPopup.IsDestroyed);
                return false;
            }

            var cameraDevice = WebCamTexture.devices[0]; // default is back camera
            if ( WebCamTexture.devices.Length > 1 ) cameraDevice = WebCamTexture.devices[1]; // front camera for mobile devices

            currentWebcamTexture = new WebCamTexture(cameraDevice.name, _requestedWidth, _requestedHeight, _requestedFps);
            currentWebcamTexture.Play();
            
            await UniTask.WaitUntil(() => currentWebcamTexture != null && currentWebcamTexture.didUpdateThisFrame);
            _trayBarVideoImage.texture = currentWebcamTexture;
            return true;
        }

        private void OnEnable()
        {
            _callListMonitor.OnShouldStartSession += ShouldStart;
            _callListMonitor.OnShouldEndSession += ShouldEnd;
        }

        private void OnDisable()
        {
            _callListMonitor.OnShouldStartSession -= ShouldStart;
            _callListMonitor.OnShouldEndSession -= ShouldEnd;
        }

        private void ShouldStart(List<VideoCallSession> listSession)
        {
            if ( listSession == null ) return;
            var mySessions = listSession.Where(videoCallSession => videoCallSession.InvolveUser(_playerSO.DatabaseId));
            var hasStartASession = false;
            foreach (var videoCallSession in mySessions)
            {
                currentCalls.TryAdd(videoCallSession._peersInfo, _callPool.Pool.Get());
                if ( !currentCalls.TryGetValue(videoCallSession._peersInfo, out var con) ) continue;
                _signalingManager.AddHandler(con._singleWebRtcConnection);
                con.SetCameraStreamerSource(currentWebcamTexture);
                con.SetReceiveCodec(_settings.ReceiverVideoCodec);
                con.SetSenderCodec(_settings.SenderVideoCodec);
                con.SetCameraStreamerSize((uint)_settings.StreamSize.x, (uint)_settings.StreamSize.y);
                con.CreateConnection(videoCallSession._peersInfo.ConnectionId);
                con.SetOrderedPeersInfo(videoCallSession._peersInfo);
                con.RegisterReceivedTexture((i, t) =>
                    AEventHandler.ExecuteEvent(GlobalEvents.OnReceivedRemoteVideo, i, t));
                _callListMonitor.SetSessionStatus(videoCallSession._peersInfo, VideoCallStatus.Started);
                hasStartASession = true;
            }

            if ( hasStartASession )
            {
                AEventHandler.ExecuteEvent(GlobalEvents.DisplayLocalVideo, currentWebcamTexture as Texture);
            }
        }

        private void ShouldEnd(List<VideoCallSession> shouldEndSessions)
        {
            if ( shouldEndSessions == null ) return;
            var mySessions =
                shouldEndSessions.Where(videoCallSession => videoCallSession.InvolveUser(_playerSO.DatabaseId));
            var aboutToRemove = new List<OrderedPeersInfo>();
            foreach (var callSession in mySessions)
            {
                if ( !currentCalls.TryGetValue(callSession._peersInfo, out var videoSingleCon) ) continue;
            
                
                videoSingleCon.DeleteConnection(callSession._peersInfo.ConnectionId);

                _signalingManager.RemoveHandler(videoSingleCon._singleWebRtcConnection);
                Debug.Log($"<color=red>End Call {callSession._peersInfo}</color>");
                _callListMonitor.SetSessionStatus(callSession._peersInfo, VideoCallStatus.Ended);
                AEventHandler.ExecuteEvent(GlobalEvents.OnCloseRemoteCamera, callSession._peersInfo);
                aboutToRemove.Add(callSession._peersInfo);
            }

            foreach (var peersInfo in aboutToRemove)
            {
                if ( peersInfo != null ) currentCalls?.Remove(peersInfo);
            }

            if ( currentCalls.Count == 0 )
            {
                AEventHandler.ExecuteEvent(GlobalEvents.CloseLocalVideo);
            }
        }
    }
}