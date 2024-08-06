using System.Collections.Generic;
using System.Linq;
using AhnLab.EventSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ConnectSphere
{
    /// <summary>
    /// Listen to the reported positions, and provide the list of OrderedPeerInfo of both peers in
    /// a specific distance
    /// </summary>
    public class DistanceScanner : PeerScanner
    {
        [SerializeField] private float _checkingInterval = 0.2f;
        [SerializeField] private float _minimalDistance = 1f;

        private readonly Dictionary<int, PeerPosition> currentPositions = new Dictionary<int, PeerPosition>();

        protected override void Awake()
        {
            base.Awake();
            AEventHandler.RegisterEvent<int, Vector2>(GlobalEvents.PlayerPositionUpdated, HandlePositionUpdated);
            AEventHandler.RegisterEvent<int>(GlobalEvents.StopPositionTracking, RemovePeersForUser);

            _ = ProcessPeersInfo();
        }

        private void OnDestroy()
        {
            AEventHandler.UnregisterEvent<int, Vector2>(GlobalEvents.PlayerPositionUpdated, HandlePositionUpdated);
            AEventHandler.UnregisterEvent<int>(GlobalEvents.StopPositionTracking, RemovePeersForUser);
        }

        private void HandlePositionUpdated(int userId, Vector2 position)
        {
            if ( currentPositions == null ) return;
            if ( currentPositions.TryGetValue(userId, out var positionedPeer) )
            {
                if ( positionedPeer != null ) positionedPeer._position = position;
            }
            else
            {
                currentPositions.Add(userId, new PeerPosition
                {
                    _userId = userId,
                    _position = position
                });
            }
        }

        private async UniTaskVoid ProcessPeersInfo()
        {
            while (currentPositions != null)
            {
                var pairWithDistance = new List<(PeerPosition, PeerPosition)>();

                foreach (var positionA in currentPositions)
                {
                    foreach (var positionB in currentPositions)
                    {
                        if ( positionA.Key == positionB.Key ) continue;
                        if ( positionA.Value == null || positionB.Value == null ) continue;
                        var distance = Vector2.Distance(positionA.Value._position, positionB.Value._position);
                        if ( !(distance <= _minimalDistance) ) continue;
                        pairWithDistance.Add((positionA.Value, positionB.Value));
                    }
                }

                var currentSets = new HashSet<OrderedPeersInfo>(pairWithDistance.Select(pair =>
                    new OrderedPeersInfo(pair.Item1._userId, pair.Item2._userId)));

                if ( !currentSets.SetEquals(_orderedPeers) )
                {
                    // var temp = new HashSet<OrderedPeersInfo>(currentSets);
                    // temp.ExceptWith(_orderedPeers);
                    //
                    // _orderedPeers.IntersectWith(currentSets);
                    // _orderedPeers.UnionWith(temp);
                    _orderedPeers = currentSets;
                    InvokePeersChanged();
                }

                await UniTask.WaitForSeconds(_checkingInterval);
            }
        }

        private class PeerPosition
        {
            public int _userId;
            public Vector2 _position;
        }
    }
}