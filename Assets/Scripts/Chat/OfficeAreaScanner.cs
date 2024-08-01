using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConnectSphere.Chat
{
    public class OfficeAreaScanner : PeerScanner
    {
        [Header("Gather Areas")] [SerializeField]
        private List<GatheringArea> _areas;

        private Dictionary<int, HashSet<OrderedPeersInfo>> _areaPeers;

        protected override void Awake()
        {
            base.Awake();
            if ( _areas == null || _areas.Count < 1 )
            {
                Debug.LogWarning($"{GetType()} does not work because it has no {typeof(GatheringArea)}");
            }

            _areaPeers = new Dictionary<int, HashSet<OrderedPeersInfo>>();
        }

        private void OnEnable()
        {
            GatheringArea.OnPlayerEnteredArea += AddOrRemovePeers;
            GatheringArea.OnPlayerExitArea += AddOrRemovePeers;
        }

        private void AddOrRemovePeers(int areaId)
        {
            if ( _areaPeers == null ) return;
            Debug.Log($"<color=yellow>|>>>> Player enter {areaId}</color>");
            if ( _areas == null || _areas.Count < 1 ) return;
            var area = _areas.FirstOrDefault(e => e != null && e.AreaId == areaId);
            if ( area == null ) return;
            var playersInArea = area.PlayersInArea;

            // all left the area that exist once in the tracked dict
            var allPlayersInAreaLeft = playersInArea == null || playersInArea.Count < 1;
            if ( allPlayersInAreaLeft && _areaPeers.TryGetValue(areaId, out var peerSet) )
            {
                foreach (var orderedPeers in peerSet)
                {
                    RemovePeers(orderedPeers);
                }

                return;
            }

            if ( allPlayersInAreaLeft ) return;
            Debug.Log($"<color=yellow>__ List Players In Area {areaId}:</color>");
            var playersId = playersInArea.Select(e => e.GetComponent<Player>().DatabaseId).ToList();
            Debug.Log($"<color=yellow>{string.Join(",", playersId)}</color>");

            var currentPeersInArea = ToOrderedPeers(playersId);
            _areaPeers.TryAdd(areaId, new HashSet<OrderedPeersInfo>());
            if ( !_areaPeers.TryGetValue(areaId, out var existingPeers) ) return;
            foreach (var p in currentPeersInArea)
            {
                if ( !existingPeers.Contains(p) ) existingPeers.Add(p);
            }

            foreach (var p in existingPeers)
            {
                if ( !currentPeersInArea.Contains(p) ) RemovePeers(p);
            }
        }


        private void OnDisable()
        {
            GatheringArea.OnPlayerEnteredArea -= AddOrRemovePeers;
            GatheringArea.OnPlayerExitArea -= AddOrRemovePeers;
        }
    }
}