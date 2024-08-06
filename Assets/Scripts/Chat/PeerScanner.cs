using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    /// <summary>
    /// Abstract class for scanning the possible connection among peers in a specific area or in office
    /// </summary>
    public abstract class PeerScanner : MonoBehaviour
    {
        public Action<HashSet<OrderedPeersInfo>> _onPeersChanged;
        protected HashSet<OrderedPeersInfo> _orderedPeers;

        protected virtual void Awake()
        {
            _orderedPeers = new HashSet<OrderedPeersInfo>();
        }

        protected void InvokePeersChanged()
        {
            _onPeersChanged?.Invoke(_orderedPeers);
        }

        protected void RemovePeers(int firstId, int secondId)
        {
            var peers = new OrderedPeersInfo(firstId, secondId);
            var done = _orderedPeers != null && _orderedPeers.Remove(peers);
            if ( done ) InvokePeersChanged();
        }

        protected void RemovePeers(OrderedPeersInfo peerInfo)
        {
            var done = _orderedPeers != null && _orderedPeers.Remove(peerInfo);
            if ( done ) InvokePeersChanged();
        }

        protected void RemovePeersForUser(int userId)
        {
            if ( _orderedPeers == null ) return;
            Debug.Log($"<color=yellow>Remove peers related to userId {userId}</color>");
            Debug.Log($"<color=yellow>Peers Before removal {string.Join(",", _orderedPeers)}</color>");
            var count = _orderedPeers.RemoveWhere(e => e != null && e.HasUser(userId));
            Debug.Log($"<color=yellow>Peers After removal {string.Join(",", _orderedPeers)}</color>");
            InvokePeersChanged();
        }

        protected void AddPeers(OrderedPeersInfo peerInfo)
        {
            var done = _orderedPeers != null && _orderedPeers.Add(peerInfo);
            if ( done ) InvokePeersChanged();
        }

        protected void AddPeers(int firstId, int secondId)
        {
            var peers = new OrderedPeersInfo(firstId, secondId);
            var done = _orderedPeers != null && _orderedPeers.Add(peers);
            if ( done ) InvokePeersChanged();
        }

        protected List<OrderedPeersInfo> ToOrderedPeers(List<int> userIds)
        {
            var pairs = new List<OrderedPeersInfo>();
            if ( userIds == null ) return pairs;
            for (var i = 0; i < userIds.Count; i++)
            {
                for (var j = i + 1; j < userIds.Count; j++)
                {
                    pairs.Add(new OrderedPeersInfo(userIds[i], userIds[j]));
                }
            }

            return pairs;
        }
    }

    public class OrderedPeersInfo
    {
        private readonly int firstPeerId;
        private readonly int secondPeerId;

        public PeerGroup PeerGroup { get; private set; }

        public OrderedPeersInfo(int first, int second)
        {
            firstPeerId = Math.Min(first, second);
            secondPeerId = Math.Max(first, second);
        }

        public void SetPeerGroup(PeerGroup pg)
        {
            PeerGroup = pg;
        }

        public string ConnectionId => $"{firstPeerId}_{secondPeerId}";

        public override int GetHashCode()
        {
            var hash = 17*31 + firstPeerId.GetHashCode();
            hash = hash*31 + secondPeerId.GetHashCode();
            return hash;
        }

        public override string ToString() => ConnectionId;

        public override bool Equals(object obj)
        {
            if ( obj is OrderedPeersInfo other )
            {
                return firstPeerId == other.firstPeerId && secondPeerId == other.secondPeerId;
            }

            return false;
        }

        public bool HasUser(int userId) => firstPeerId == userId || secondPeerId == userId;
    }

    public enum PeerGroup
    {
        InOffice,
        OutOfOffice
    }
}