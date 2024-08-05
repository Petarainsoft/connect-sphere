using System;
using System.Collections.Generic;
using System.Linq;
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
            if ( done ) _onPeersChanged?.Invoke(_orderedPeers);
        }

        protected void RemovePeers(OrderedPeersInfo peerInfo)
        {
            var done = _orderedPeers != null && _orderedPeers.Remove(peerInfo);
            if ( done ) _onPeersChanged?.Invoke(_orderedPeers);
        }

        protected void RemovePeersRelatedTo(int id)
        {
            Debug.Log($"<color=yellow>Remove peers related to id {id}</color>");
            Debug.Log($"<color=yellow>Peers Before removal {string.Join(",", _orderedPeers)}</color>");
            var count = _orderedPeers.RemoveWhere(e => e.HasUser(id));
            // if ( count > 0 )
            // {0
            Debug.Log($"<color=yellow>Peers After removal {string.Join(",", _orderedPeers)}</color>");
            _onPeersChanged?.Invoke(_orderedPeers);
            // }
        }

        protected void AddPeers(OrderedPeersInfo peerInfo)
        {
            var done = _orderedPeers != null && _orderedPeers.Add(peerInfo);
            if ( done ) _onPeersChanged?.Invoke(_orderedPeers);
        }

        protected void AddPeers(int firstId, int secondId)
        {
            var peers = new OrderedPeersInfo(firstId, secondId);
            var done = _orderedPeers != null && _orderedPeers.Add(peers);
            if ( done ) _onPeersChanged?.Invoke(_orderedPeers);
        }

        protected List<OrderedPeersInfo> ToOrderedPeers(List<int> elements)
        {
            var pairs = new List<OrderedPeersInfo>();

            for (var i = 0; i < elements.Count; i++)
            {
                for (var j = i + 1; j < elements.Count; j++)
                {
                    pairs.Add(new OrderedPeersInfo(elements[i], elements[j]));
                }
            }

            return pairs;
        }
    }

    public class OrderedPeersInfo
    {
        private readonly int firstPeerId;
        private readonly int secondPeerId;

        private PeerGroup peerGroup;

        public OrderedPeersInfo(int first, int second)
        {
            firstPeerId = Math.Min(first, second);
            secondPeerId = Math.Max(first, second);
        }

        public void SetPeerGroup(PeerGroup pg)
        {
            peerGroup = pg;
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