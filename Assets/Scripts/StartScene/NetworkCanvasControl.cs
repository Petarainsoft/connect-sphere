using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Netcode.Transports.UTP;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace ConnectSphere
{
    public class NetworkCanvasControl : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField _inputIpAddress;
        [SerializeField] private TMP_InputField _inputConnectionPort;
        [SerializeField] private TMP_Text _textConnectionStatus;
        [SerializeField] private Button _buttonHost;
        [SerializeField] private Button _buttonServer;
        [SerializeField] private Button _buttonClient;
        [SerializeField] private Canvas _selectionCanvas;
        [SerializeField] private Canvas _networkCanvas;

        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfoSo;

        private NetworkManager _networkManager;
        private UnityTransport _transport;

        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            _transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        }

        private void Start()
        {
            ShowNetworkMenu();

            _networkManager.OnConnectionEvent += OnConnectionEvent;
            _buttonHost.onClick.AddListener(OnHostButtonClicked);
            _buttonServer.onClick.AddListener(OnServerButtonClicked);
            _buttonClient.onClick.AddListener(OnClientButtonClicked);
        }

        private void OnDestroy()
        {
            _networkManager.OnConnectionEvent -= OnConnectionEvent;
            if (_buttonHost != null)
            {
                _buttonHost.onClick.RemoveListener(OnHostButtonClicked);
            }

            if (_buttonServer != null)
            {
                _buttonServer.onClick.RemoveListener(OnServerButtonClicked);
            }

            if (_buttonClient != null)
            {
                _buttonClient.onClick.RemoveListener(OnClientButtonClicked);
            }
        }

        void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {
            if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
            {
                ShowSelectionMenu();
            }
            else if (connectionEventData.EventType == ConnectionEvent.ClientDisconnected)
            {
                if ((NetworkManager.Singleton.IsServer && connectionEventData.ClientId != NetworkManager.ServerClientId))
                {
                    return;
                }
                ShowNetworkMenu();
            }
        }

        private void ToggleButtons(bool toggle)
        {
            _buttonHost.interactable = toggle;
            _buttonServer.interactable = toggle;
            _buttonClient.interactable = toggle;
        }    

        public void OnHostButtonClicked()
        {
            _playerInfoSo.Role = NetworkRole.Host;
            if (SetConnectionData())
            {
                ShowSelectionMenu();
            }
        }

        public void OnServerButtonClicked()
        {
            _playerInfoSo.Role = NetworkRole.Server;
            if (SetConnectionData())
            {
                ShowSelectionMenu();
            }
        }

        public void OnClientButtonClicked()
        {
            _playerInfoSo.Role = NetworkRole.Client;
            if (SetConnectionData())
            {
                StartCoroutine(ShowConnectingStatus());
                ShowSelectionMenu();
            }
        }

        private bool SetConnectionData()
        {
            if (_inputIpAddress.text == "")
            {
                _textConnectionStatus.text = "IP Address Invalid";
                return false;
            }

            if (_inputConnectionPort.text == "")
            {
                _textConnectionStatus.text = "Port Invalid";
                return false;
            }

            if (ushort.TryParse(_inputConnectionPort.text, out ushort port))
            {
                _transport.SetConnectionData(_inputIpAddress.text, port);
            }
            else
            {
                _transport.SetConnectionData(_inputIpAddress.text, 7777);
            }
            return true;
        }

        string Sanitize(string dirtyString)
        {
            // sanitize the input for the ip address
            return Regex.Replace(dirtyString, "[^A-Za-z0-9.]", "");
        }

        private void ShowStatusText(bool visible)
        {
            _textConnectionStatus.enabled = visible;
        }

        IEnumerator ShowConnectingStatus()
        {
            _textConnectionStatus.text = "Attempting to Connect...";
            ShowStatusText(true);
            ToggleButtons(false);

            yield return new WaitForSeconds(_transport.ConnectTimeoutMS * _transport.MaxConnectAttempts / 1000f);

            // wait to verify connect status
            yield return new WaitForSeconds(1f);

            _textConnectionStatus.text = "Connection Attempt Failed";
            ToggleButtons(true);

            yield return new WaitForSeconds(3f);

            ShowStatusText(false);
        }

        void ConnectionApprovalWithRandomSpawnPos(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            // Here we are only using ConnectionApproval to set the player's spawn position. Connections are always approved.
            response.CreatePlayerObject = true;
            response.Position = Vector3.zero;
            response.Rotation = Quaternion.identity;
            response.Approved = true;
        }

        private void ShowSelectionMenu()
        {
            _selectionCanvas.sortingOrder = 1;
            _networkCanvas.sortingOrder = 0;
        }

        private void ShowNetworkMenu()
        {
            _networkCanvas.sortingOrder = 1;
            _selectionCanvas.sortingOrder = 0;
        }
    }
}
