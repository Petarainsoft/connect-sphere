using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace ConnectSphere
{
    public class GameManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfoSo;

        [Header("Prefabs")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private List<GameObject> _characterModelsPrefabs;

        [Header("Objects")]
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        private Vector2 _spawnPosition = Vector2.zero;

        private void Start()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            GameObject playerObject = Instantiate(_playerPrefab, _spawnPosition, Quaternion.identity);
            GameObject characterObject = Instantiate(_characterModelsPrefabs[_playerInfoSo.AvatarIndex], playerObject.transform);
            playerObject.GetComponent<Player>().SetName(_playerInfoSo.PlayerName);
            playerObject.GetComponent<PlayerInput>().SetupComponents();
            _virtualCamera.Follow = playerObject.transform;
        }
    }
}
