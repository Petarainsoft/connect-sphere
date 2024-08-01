using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConnectSphere
{
    public class VideoCallListMonitor : MonoBehaviour
    {
        private List<PeerScanner> scanners;
        [SerializeField] private Dictionary<Ordered2Peers, VideoCallSession> allSessions;

        public Action<List<VideoCallSession>> OnShouldStartSession;
        public Action<List<VideoCallSession>> OnShouldEndSession;

        private void Awake()
        {
            scanners = new List<PeerScanner>(GetComponents<PeerScanner>());
            if ( scanners.Count < 1 )
            {
                Debug.LogWarning($"{GetType()} has no {typeof(PeerScanner)}");
                return;
            }
        }

        private void OnEnable()
        {
            foreach (var scanner in scanners)
            {
                scanner._onPeersChanged += HandlePeerList;
            }
        }

        public void SetPeerStarted(Ordered2Peers inCallPeer)
        {
            if ( allSessions.TryGetValue(inCallPeer, out var inCall) )
            {
                inCall._status = VideoCallStatus.Started;
                Debug.Log($"<color=green>\t{inCall} started </color>");
            }
        }

        public void SetPeerEnded(Ordered2Peers endedPeers)
        {
            if ( allSessions.ContainsKey(endedPeers) )
            {
                allSessions[endedPeers]._status = VideoCallStatus.Ended;
            }
        }

        private void HandlePeerList(HashSet<Ordered2Peers> currentPeers)
        {
            if ( currentPeers == null || currentPeers.Count < 1 ) return;
            var logPeers = string.Join(",", currentPeers);
            // Debug.Log("<color=red>CurrentPeers</color>");
            Debug.Log($"<color=yellow>{logPeers}</color>");
            // the list is brand new
            if ( allSessions == null || allSessions.Count < 1 )
            {
                InitSession(currentPeers);
                return;
            }

            var currentWorkingPeers = allSessions
                .Where(e => e.Value._status == VideoCallStatus.Started || e.Value._status == VideoCallStatus.ShouldEnd)
                .Select(e => e.Value._peers).ToHashSet();
            var endedPeers = allSessions
                .Where(e => e.Value._status == VideoCallStatus.Ended)
                .Select(e => e.Value._peers).ToHashSet();

            var newPeers = currentPeers.Except(currentWorkingPeers);
            var shouldEndPeers = currentWorkingPeers.Except(currentPeers);

            foreach (var newPeer in newPeers)
            {
                var videoCallSession = new VideoCallSession()
                {
                    _peers = newPeer,
                    _status = VideoCallStatus.ShouldStart
                };
                allSessions.TryAdd(newPeer, videoCallSession);
            }
            
            var endedPeersButShouldStart = currentPeers.Intersect(endedPeers);
            foreach (var p in endedPeersButShouldStart)
            {
                allSessions[p]._status = VideoCallStatus.ShouldStart;
            }

            // handle shouldStart Session
            var shouldStart = allSessions.Where(e => e.Value._status == VideoCallStatus.ShouldStart)
                .Select(e => e.Value).ToList();
            OnShouldStartSession?.Invoke(shouldStart);


            // Handle shouldEnd session
            var shouldEndConnections = new List<VideoCallSession>();
            foreach (var shouldEnd in shouldEndPeers)
            {
                allSessions[shouldEnd]._status = VideoCallStatus.ShouldEnd;
            }

            shouldEndConnections.AddRange(
                allSessions.Where(e => e.Value._status == VideoCallStatus.ShouldEnd)
                    .Select(e => e.Value));

            OnShouldEndSession?.Invoke(shouldEndConnections);
        }


        private void InitSession(HashSet<Ordered2Peers> initSet)
        {
            foreach (var peersPeer in initSet)
            {
                allSessions ??= new Dictionary<Ordered2Peers, VideoCallSession>();
                allSessions.TryAdd(peersPeer, new VideoCallSession()
                {
                    _peers = peersPeer,
                    _status = VideoCallStatus.ShouldStart
                });
            }
        }

        private void OnDisable()
        {
            foreach (var scanner in scanners)
            {
                scanner._onPeersChanged -= HandlePeerList;
            }
        }
    }

    public enum VideoCallStatus
    {
        ShouldStart,
        Started,
        ShouldEnd,
        Ended
    }

    public class VideoCallSession
    {
        public Ordered2Peers _peers;
        public VideoCallStatus _status;

        public override bool Equals(object obj)
        {
            if ( obj is VideoCallSession other )
            {
                return _peers == other._peers;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _peers.GetHashCode();
        }

        public override string ToString()
        {
            return _peers + "(" + _status + ")";
        }

        public bool RelateTo(int userId) => _peers.RelateTo(userId);
    }
}