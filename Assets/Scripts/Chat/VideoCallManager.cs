using System;
using System.Collections;
using System.Collections.Generic;
using Best.HTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using Chat.States;
using ConnectSphere;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using ParrelSync;
#endif
using TMPro;
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
        [SerializeField] private SignalingManager _renderStreaming;
        [SerializeField] private RawImage _trayBarVideoImage;
        [SerializeField] private List<RawImage> _remoteVideoImages;


        [SerializeField] private VideoCallListMonitor _callListMonitor;
        [SerializeField] private ConnectionPool _callPool;

        private Dictionary<Ordered2Peers, VideoSingleConnection> _currentCalls =
            new Dictionary<Ordered2Peers, VideoSingleConnection>();

        [SerializeField] private RenderStreamingSettings _settings;

        [SerializeField] private PlayerInfoSO _playerSO;

        private WebCamTexture webcamTexture;
        private const float WaitTime = 2f;

        [SerializeField] private Texture2D _black;


        private PermissionHelper _cameraPermissionHelper = new PermissionHelper(Permission.Camera);

        private void AskPermissionVideo()
        {
            _cameraPermissionHelper.OnPermissionResult = AfterRequestPermission;
            _cameraPermissionHelper.Ask();
        }

        private void AfterRequestPermission(bool requestOk)
        {
            Debug.Log($"Request Permission with {requestOk}");
            if ( requestOk )
            {
                RequestCameraPermission();
            }
        }

        public void ToggleCamera(bool isOff)
        {
            if ( !isOff )
            {
                AskPermissionVideo();
            }
            else
            {
                webcamTexture.Stop();
                _trayBarVideoImage.texture = _black;
            }
        }

        private async void RequestCameraPermission()
        {
            if ( _trayBarVideoImage == null )
            {
                Debug.LogError("Texture for showing camera is null");
                return;
            }

            await UniTask.WaitUntil(() => WebCamTexture.devices != null && WebCamTexture.devices.Length >= 1);

            if ( WebCamTexture.devices == null || WebCamTexture.devices.Length == 0 )
            {
                return;
            }

            var cameraDevice = WebCamTexture.devices[0];
            if ( WebCamTexture.devices.Length > 1 )
                cameraDevice = WebCamTexture.devices[1]; // front camera for mobile devices

            webcamTexture = new WebCamTexture(cameraDevice.name, 600, 480, (int)20);
            webcamTexture.Play();

            await UniTask.WaitUntil(() => webcamTexture != null && webcamTexture.didUpdateThisFrame);
            Debug.Log($"CameraTexture Size w:{webcamTexture.width} h{webcamTexture.height}");

            _trayBarVideoImage.texture = webcamTexture;
        }


        private async UniTask<bool> RequestCamera()
        {
            if ( _trayBarVideoImage == null )
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
            if ( WebCamTexture.devices.Length > 1 )
            {
                cameraDevice = WebCamTexture.devices[1]; // front camera for mobile devices
            }

            webcamTexture = new WebCamTexture(cameraDevice.name, 600, 480, (int)20);
            webcamTexture.Play();

            await UniTask.WaitUntil(() => webcamTexture != null && webcamTexture.didUpdateThisFrame);
            Debug.Log($"CameraTexture Size w:{webcamTexture.width} h{webcamTexture.height}");

            _trayBarVideoImage.texture = webcamTexture;

            Debug.Log($"Container Size w:{_trayBarVideoImage.texture.width} h{_trayBarVideoImage.texture.height}");

            await UniTask.WaitForEndOfFrame(this);
            await UniTask.WaitForSeconds(1f);

            return true;
        }

        public void OnTextureReceive(Texture receiveTexture, int textureIndex)
        {
            Debug.Log($"Receive Texture for index {textureIndex}");
            _remoteVideoImages[textureIndex].texture = receiveTexture;
            _remoteVideoImages[textureIndex].transform.parent.gameObject.SetActive(true);
        }

        private IEnumerator Start()
        {
            if ( _renderStreaming.runOnAwake )
                yield break;
            if ( _settings != null )
                _renderStreaming.useDefaultSettings = _settings.UseDefaultSettings;
            if ( _settings?.SignalingSettings != null )
                _renderStreaming.SetSignalingSettings(_settings.SignalingSettings);
            // _renderStreaming.Run();

            RequestCamera();
            Debug.Log("RenderingStreaming is running");
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
            var logs = string.Join("," , listSession);
            // Debug.Log($"<color=green>{logs}</color>");
            foreach (var videoCallSession in listSession)
            {
                if ( !videoCallSession.RelateTo(1) ) continue;

                // _currentCalls.TryAdd(videoCallSession._peers, _callPool.Pool.Get());
                // if ( _currentCalls.TryGetValue(videoCallSession._peers, out var connection) )
                // {
                //     connection.SetReceiveCodec(_settings.ReceiverVideoCodec);
                //     connection.SetSenderCodec(_settings.SenderVideoCodec);
                //     connection.SetCameraStreamerSource(_trayBarVideoImage.texture);
                //     connection.SetCameraStreamerSize((uint)_settings.StreamSize.x, (uint)_settings.StreamSize.y);
                //     connection.CreateConnection(videoCallSession._peers.ConnectionId);
                // }

                Debug.Log($"<color=green>Start Call {videoCallSession._peers}</color>");
                _callListMonitor.SetPeerStarted(videoCallSession._peers);
            }
        }

        private void ShouldEnd(List<VideoCallSession> listSession)
        {
            var logs = string.Join("," , listSession);
            // Debug.Log($"<color=red>{logs}</color>");
            foreach (var videoCallSession in listSession)
            {
                if ( !videoCallSession.RelateTo(1) ) continue;

                // if ( _currentCalls.TryGetValue(videoCallSession._peers, out var connection) )
                // {
                //     connection.DeleteConnection(videoCallSession._peers.ConnectionId);
                // }
                
                Debug.Log($"<color=red>End Call {videoCallSession._peers}</color>");
                _callListMonitor.SetPeerEnded(videoCallSession._peers);
            }
        }
    }
}