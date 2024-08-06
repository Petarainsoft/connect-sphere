using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConnectSphere.Chat
{
    public class OfficeAreaScanner : PeerScanner
    {
        [SerializeField] private PlayerInfoSO _playerInfoSo;
        private void OnEnable()
        {
            GatheringArea.OnPlayerEnteredArea += OnEnterArea;
            GatheringArea.OnPlayerExitArea += OnExitArea;
        }

        private void OnEnterArea(int areaId, int userId, List<int> userInArea)
        {
            // things happen in other area
            if (userId != _playerInfoSo.DatabaseId && !userInArea.Contains(_playerInfoSo.DatabaseId)) return;
            var peers = ToOrderedPeers(userInArea);
            if ( _orderedPeers.SetEquals(peers) ) return;
            _orderedPeers = peers.ToHashSet();
            InvokePeersChanged();
        }

        private void OnExitArea(int areaId, int userId, List<int> userInArea)
        {
            // things happen in other area
            if (userId != _playerInfoSo.DatabaseId && !userInArea.Contains(_playerInfoSo.DatabaseId))
            {
                return;
            }

            if ( userId == _playerInfoSo.DatabaseId ) // me leaving
            {
                RemovePeersForUser(userId);
                return;
            }
            // other left, and I am still in the area
            var peers = ToOrderedPeers(userInArea);
            if ( _orderedPeers.SetEquals(peers) ) return;
            _orderedPeers = peers.ToHashSet();
            InvokePeersChanged();
        }

        private void OnDisable()
        {
            GatheringArea.OnPlayerEnteredArea -= OnEnterArea;
            GatheringArea.OnPlayerExitArea -= OnExitArea;
        }
    }
}