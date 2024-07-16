using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Common;
using Fusion;

namespace ConnectSphere
{
    public class GameManager : NetworkBehaviour
    {
        enum GamePhase
        {
            Starting,
            Running,
            Ending
        }

        [Networked] private GamePhase Phase { get; set; }
        private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

        private void Awake()
        {
            GetComponent<NetworkObject>().Flags |= NetworkObjectFlags.MasterClientObject;
        }

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                // Initialize the game state on the master client
                Phase = GamePhase.Starting;
            }
        }

        public override void Render()
        {
            switch (Phase)
            {
                case GamePhase.Starting:
                    UpdateStarter();
                    break;
                case GamePhase.Running:
                    break;
                case GamePhase.Ending:
                    break;
            }
        }

        private void UpdateStarter()
        {
            if (!Object.HasStateAuthority)
                return;

            FindObjectOfType<PlayerSpawner>().StartPlayerSpawner();
            Phase = GamePhase.Running;
        }

        public void TrackNewPlayer(NetworkBehaviourId playerDataNetworkedId)
        {
            _playerDataNetworkedIds.Add(playerDataNetworkedId);
        }
    }
}
