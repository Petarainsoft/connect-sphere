using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class Activity : NetworkBehaviour
    {
        [Networked, Capacity(2)] public NetworkLinkedList<int> InteractingPlayers { get; }
        [SerializeField] private ActivityController _activityController;
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
            _activityController.FillPlayers(InteractingPlayers);
            _activityController.gameObject.SetActive(true);
        }
    }
}
