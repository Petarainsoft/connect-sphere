using System.Collections;
using Chat.States;
using TMPro;
using Unity.RenderStreaming;

namespace Chat
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    
    /// <summary>
    /// Handle both video streaming using webrtc and audio using vivox
    /// </summary>

    [RequireComponent(typeof(VivoxVideoCallFsm))]
    public class VivoxVideoCall : MonoBehaviour
    {
        [SerializeField] private SignalingManager renderStreaming;
        [SerializeField] private TMP_Dropdown webcamSelectDropdown;
        [SerializeField] private TMP_Dropdown microphoneSelectDropdown;
        [SerializeField] private Button startButton;
        [SerializeField] private Button setUpButton;
        [SerializeField] private Button hangUpButton;
        [SerializeField] private InputField connectionIdInput;
        [SerializeField] private RawImage localVideoImage;
        [SerializeField] private RawImage remoteVideoImage;
        [SerializeField] private VideoStreamSender webCamStreamer;
        [SerializeField] private VideoStreamReceiver receiveVideoViewer;
        [SerializeField] private SingleConnection singleConnection;

        [SerializeField] private VivoxServiceHelper _vivoxHelper;
        
        

        private string connectionId;
        
        [SerializeField]
        private RenderStreamingSettings settings;
        
        private VivoxVideoCallFsm fsm;
        
        WebCamTexture m_webcamTexture;

        private IEnumerator ShowLocalCamera()
        {
            yield return new WaitUntil(() => WebCamTexture.devices != null && WebCamTexture.devices.Length > 1);
            WebCamDevice userCameraDevice = WebCamTexture.devices[1];
            m_webcamTexture = new WebCamTexture(userCameraDevice.name,1280, 720
                , (int)30);
            m_webcamTexture.Play();
            yield return new WaitUntil(() => m_webcamTexture.didUpdateThisFrame);
            Debug.Log($"CameraTexture Size w:{m_webcamTexture.width} h{m_webcamTexture.height}");

            localVideoImage.texture = m_webcamTexture;
            Debug.Log($"Container Size w:{localVideoImage.texture.width} h{localVideoImage.texture.height}");
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
            
            startButton.interactable = true;
            webcamSelectDropdown.interactable = true;
            setUpButton.interactable = false;
            hangUpButton.interactable = false;
            connectionIdInput.interactable = true;
            // startButton.onClick.AddListener(() => { StartLocalVideo(); });
            setUpButton.onClick.AddListener(SetUp);
            hangUpButton.onClick.AddListener(HangUp);
            connectionIdInput.onValueChanged.AddListener(input => connectionId = input);
            connectionIdInput.text = $"{Random.Range(0, 99999):D5}";
            webcamSelectDropdown.onValueChanged.AddListener(index => webCamStreamer.sourceDeviceIndex = index);
            webcamSelectDropdown.options = WebCamTexture.devices.Select(x => x.name)
                .Select(x => new TMP_Dropdown.OptionData(x)).ToList();
            webCamStreamer.OnStartedStream += id => receiveVideoViewer.enabled = true;
            webCamStreamer.OnStartedStream += _ =>
            {
                Debug.Log("*** VIVOX START LOGIN AND JOIN");
                _vivoxHelper.LoginVivoxAndJoinRoom(connectionId);
                Debug.Log("*** VIVOX START LOGIN AND JOIN 2");
                localVideoImage.texture = webCamStreamer.sourceWebCamTexture;
            };

            if ( settings != null )
            {
                webCamStreamer.width = (uint)settings.StreamSize.x;
                webCamStreamer.height = (uint)settings.StreamSize.y;
                
                settings.ApplyH264Codec();
            }

            receiveVideoViewer.OnUpdateReceiveTexture += texture =>
            {
                localVideoImage.GetComponent<ToggleFullscreenUI>()?.SetFullScreen(false);
                remoteVideoImage.texture = texture;
            };
        }

        public void StartLocalCamera()
        {
            webCamStreamer.enabled = true;
            startButton.interactable = false;
            webcamSelectDropdown.interactable = false;
            setUpButton.interactable = true;
            StartCoroutine(ShowLocalCamera());
        }

        private IEnumerator Start()
        {
            setUpButton.interactable = false;
            hangUpButton.interactable = false;
            connectionIdInput.interactable = false;

            yield return new WaitUntil(() => VivoxVoiceManager.Instance != null && VivoxVoiceManager.Instance.IsReady);
            Debug.Log("VivoxVoiceManager is ready");
            yield return new WaitUntil(() => _vivoxHelper != null && _vivoxHelper.IsReady);
            Debug.Log("VivoxServiceHelper is ready");
            
            if ( renderStreaming.runOnAwake )
                yield break;
            if ( settings != null )
                renderStreaming.useDefaultSettings = settings.UseDefaultSettings;
            if ( settings?.SignalingSettings != null )
                renderStreaming.SetSignalingSettings(settings.SignalingSettings);
            renderStreaming.Run();
            Debug.Log("RenderingStreaming is running");
            setUpButton.interactable = true;
            hangUpButton.interactable = true;
            connectionIdInput.interactable = true;
        }

        private void SetUp()
        {
            setUpButton.interactable = false;
            hangUpButton.interactable = true;
            connectionIdInput.interactable = false;

            if ( settings != null )
            {
                receiveVideoViewer.SetCodec(settings.ReceiverVideoCodec);
                webCamStreamer.SetCodec(settings.SenderVideoCodec);
            }

            singleConnection.CreateConnection(connectionId);
        }

        private void HangUp()
        {
            singleConnection.DeleteConnection(connectionId);

            remoteVideoImage.texture = null;
            setUpButton.interactable = true;
            hangUpButton.interactable = false;
            connectionIdInput.interactable = true;
            connectionIdInput.text = $"{Random.Range(0, 99999):D5}";
            localVideoImage.texture = null;
            localVideoImage.GetComponent<ToggleFullscreenUI>()?.SetFullScreen(true);
            
            _vivoxHelper.LogoutOfVivoxServiceAsync();
        }
    }
}