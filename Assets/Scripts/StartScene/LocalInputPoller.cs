using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace ConnectSphere
{
    public class LocalInputPoller : MonoBehaviour, INetworkRunnerCallbacks
    {
        private const string AXIS_HORIZONTAL = "Horizontal";
        private const string AXIS_VERTICAL = "Vertical";
        private const string BUTTON_INTERACT = "Interact";
        private const string BUTTON_OPEN_USER_INFO = "OpenUserInfo";
        private const string BUTTON_INVITE_TO_GAME = "InviteToGame";

        // The INetworkRunnerCallbacks of this LocalInputPoller are automatically detected
        // because the script is located on the same object as the NetworkRunner and
        // NetworkRunnerCallbacks scripts.

        private bool blockInput = false;
        private void Awake()
        {
            TextChatUI.OnTyping += MustBlockInput;
        }

        void MustBlockInput(bool blocked)
        {
            blockInput = blocked;
        }
        private void OnDestroy()
        {
            TextChatUI.OnTyping -= MustBlockInput;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if ( blockInput ) return;
            PlayerInput localInput = new PlayerInput();

            localInput.HorizontalInput = Input.GetAxisRaw(AXIS_HORIZONTAL);
            localInput.VerticalInput = Input.GetAxisRaw(AXIS_VERTICAL);
            localInput.Buttons.Set(PlayerButtons.Interact, Input.GetButton(BUTTON_INTERACT));
            localInput.Buttons.Set(PlayerButtons.OpenUserInfo, Input.GetButton(BUTTON_OPEN_USER_INFO));
            localInput.Buttons.Set(PlayerButtons.InviteToGame, Input.GetButton(BUTTON_INVITE_TO_GAME));

            input.Set(localInput);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}
