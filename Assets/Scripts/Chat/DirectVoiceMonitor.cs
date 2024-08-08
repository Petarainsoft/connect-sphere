using System.Collections.Generic;
using System.Linq;
using Chat;
using UnityEngine;

namespace ConnectSphere.Chat
{
    public class DirectVoiceMonitor : MonoBehaviour
    {
        [SerializeField] private float _checkingInterval = 0.2f;
        [SerializeField]
        private DistanceScanner _distanceScanner;
        private VivoxServiceHelper _vivoxHelper;

        private void OnEnable()
        {
            _distanceScanner._onPeersChanged += HandlePeerList;
        }
        
        private void OnDisable()
        {
            _distanceScanner._onPeersChanged -= HandlePeerList;
        }
        
        private void HandlePeerList(HashSet<OrderedPeersInfo> peers)
        {
            foreach (var peer in peers)
            {
                _vivoxHelper.JoinDirectVoiceCall(peer.ConnectionId);
            }

            foreach (var activeChannel in _vivoxHelper.GetDirectCallActiveChannels())
            {
                if (peers.All(p => p.ConnectionId != activeChannel))
                {
                    _vivoxHelper.LeaveDirectVoiceCall(activeChannel);
                }
            }
        }

    }
}