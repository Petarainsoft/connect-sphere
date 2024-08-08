using AccountManagement;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConnectSphere
{
    public class OnServerDisconnected : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private string _menuSceneName = "ConnectSphere-Menu";

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (PlayerPrefs.GetInt("back") == 1)
                BackAsync();
            else
            {
                SignServicesOutAsync();
            }
        }

        private async void SignServicesOutAsync()
        {
            if (VivoxService.Instance != null)
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
                await VivoxService.Instance.LogoutAsync();
            }

            // if ( ApiManager.Instance != null ) ApiManager.Instance.Logout();
            if (AuthenticationService.Instance != null)
                AuthenticationService.Instance.SignOut();

            SceneManager.LoadScene(_menuSceneName);
        }

        private async void BackAsync()
        {
            if (VivoxService.Instance != null)
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
                await VivoxService.Instance.LogoutAsync();
            }

            SceneManager.LoadScene(_menuSceneName);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            _ = JoinOfficeSuccess();
        }
        
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

        public void OnConnectRequest(
            NetworkRunner runner,
            NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token
        ) { }

        public void OnConnectFailed(
            NetworkRunner runner,
            NetAddress remoteAddress,
            NetConnectFailedReason reason
        ) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(
            NetworkRunner runner,
            Dictionary<string, object> data
        ) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnReliableDataReceived(
            NetworkRunner runner,
            PlayerRef player,
            ReliableKey key,
            ArraySegment<byte> data
        ) { }

        public void OnReliableDataProgress(
            NetworkRunner runner,
            PlayerRef player,
            ReliableKey key,
            float progress
        ) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }


        private async UniTaskVoid JoinOfficeSuccess()
        {
            OfficeApiHandler officeLoader = ApiManager.Instance.GetComponent<OfficeApiHandler>();
            await officeLoader.CreateNewOffice(PlayerPrefs.GetString("office"), "/office-1");
            await officeLoader.UpdateLastAccess(PlayerPrefs.GetString("office"), PlayerPrefs.GetString("username"));

        }
    }
}
