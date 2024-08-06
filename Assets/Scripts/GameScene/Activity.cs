using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class Activity : NetworkBehaviour
    {
        [Networked, Capacity(2)] public NetworkLinkedList<int> InteractingPlayers { get; }
        [SerializeField] private ActivityController _activityControllerPrefab;
        [SerializeField] private Transform _activityCanvas;
        [SerializeField] private BooleanEventHandlerSO _onActivityPanelToggled;

        public void AddPlayer(int p)
        {
            AddPlayerRpc(p);
        }

        public void RemovePlayer(int p)
        {
            RemovePlayerRpc(p);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void AddPlayerRpc(int playerId)
        {
            InteractingPlayers.Add(playerId);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RemovePlayerRpc(int playerId)
        {
            InteractingPlayers.Remove(playerId);
        }

        public void StartActivity()
        {
            _onActivityPanelToggled.RaiseEvent(true);
            ActivityController controller = Runner.Spawn(_activityControllerPrefab);
            controller.transform.SetParent(_activityCanvas);
            controller.FillPlayers(InteractingPlayers);
        }
    }
}
