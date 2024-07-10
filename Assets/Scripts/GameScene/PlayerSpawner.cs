using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using Cinemachine;
using Common;

namespace ConnectSphere
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfoSo;

        [Header("Prefabs")]
        [SerializeField] private NetworkObject _playerPrefab;
        [SerializeField] private List<GameObject> _characterSpritesPrefabs;

        [Header("Objects")]
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private DynamicJoystick _joystick;
        [Networked] private bool _gameIsReady { get; set; } = false;

        private Vector2 _spawnPosition = Vector2.zero;
        private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

        public override void Spawned()
        {
            if (_gameIsReady)
            {
                SpawnPlayer(Runner.LocalPlayer);
            }
        }

        public void StartPlayerSpawner()
        {
            _gameIsReady = true;
            InitialPlayerSpawnRpc();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void InitialPlayerSpawnRpc()
        {
            SpawnPlayer(Runner.LocalPlayer);
        }

        private void SpawnPlayer(PlayerRef player)
        {
            NetworkObject playerObject = Runner.Spawn(_playerPrefab, _spawnPosition, Quaternion.identity, player);
            Runner.SetPlayerObject(player, playerObject);
            _joystick.OnJoystickValueChanged.AddListener(playerObject.GetComponent<PlayerController>().SetMovement);
            _virtualCamera.Follow = playerObject.transform;
        }

        private void OnDestroy()
        {
            _joystick.OnJoystickValueChanged.RemoveAllListeners();
        }
    }
}
