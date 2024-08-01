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
        public Action<HashSet<Ordered2Peers>> _onPeersChanged;
        protected HashSet<Ordered2Peers> _orderedPeers;

        protected virtual void Awake()
        {
            _orderedPeers = new HashSet<Ordered2Peers>();
        }

        protected void RemovePeers(int firstId, int secondId)
        {
            var peers = new Ordered2Peers(firstId, secondId);
            var done = _orderedPeers != null && _orderedPeers.Remove(peers);
            if (done) _onPeersChanged?.Invoke(_orderedPeers);
        }

        protected void RemovePeers(Ordered2Peers peer)
        {
            var done = _orderedPeers != null && _orderedPeers.Remove(peer);
            if (done) _onPeersChanged?.Invoke(_orderedPeers);
        }
        
        protected void AddPeers(Ordered2Peers peer)
        {
            var done = _orderedPeers != null && _orderedPeers.Add(peer);
            if (done) _onPeersChanged?.Invoke(_orderedPeers);
        }

        protected void AddPeers(int firstId, int secondId)
        {
            var peers = new Ordered2Peers(firstId, secondId);
            var done = _orderedPeers != null && _orderedPeers.Add(peers);
            if (done) _onPeersChanged?.Invoke(_orderedPeers);
        }
        
        protected List<Ordered2Peers> ToOrderedPeers(List<int> elements)
        {
            var pairs = new List<Ordered2Peers>();

            for (int i = 0; i < elements.Count; i++)
            {
                for (int j = i + 1; j < elements.Count; j++)
                {
                    pairs.Add(new Ordered2Peers(elements[i], elements[j]));
                }
            }

            return pairs;
        }
    }

    public class Ordered2Peers
    {
        private readonly int firstPeerId;
        private readonly int secondPeerId;

        public Ordered2Peers(int first, int second)
        {
            firstPeerId = Math.Min(first, second);
            secondPeerId = Math.Max(first, second);
        }

        public string ConnectionId => $"{firstPeerId}_{secondPeerId}";

        public override int GetHashCode()
        {
            var hash = 17 * 31 + firstPeerId.GetHashCode();
            hash = hash * 31 + secondPeerId.GetHashCode();
            return hash;
        }

        public override string ToString() => ConnectionId;

        public override bool Equals(object obj)
        {
            if (obj is Ordered2Peers other)
            {
                return firstPeerId == other.firstPeerId && secondPeerId == other.secondPeerId;
            }
            return false;
        }

        public bool RelateTo(int userId) => firstPeerId == userId || secondPeerId == userId;
    }
}
