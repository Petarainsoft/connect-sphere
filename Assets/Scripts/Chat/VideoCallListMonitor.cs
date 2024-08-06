using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConnectSphere
{
    /// <summary>
    /// Manage list peers scanners, report the list of video sessions that could be ended of started.
    /// </summary>
    public class VideoCallListMonitor : MonoBehaviour
    {
        private List<PeerScanner> scanners;
        private Dictionary<OrderedPeersInfo, VideoCallSession> allSessions;

        public Action<List<VideoCallSession>> OnShouldStartSession;
        public Action<List<VideoCallSession>> OnShouldEndSession;

        private void Awake()
        {
            scanners = new List<PeerScanner>(GetComponents<PeerScanner>() ?? Array.Empty<PeerScanner>());
            if ( scanners.Count < 1 ) Debug.LogWarning($"{GetType()} has no {typeof(PeerScanner)}");
        }

        private void OnEnable()
        {
            if ( scanners == null ) return;
            foreach (var scanner in scanners)
            {
                if ( scanner != null ) scanner._onPeersChanged += HandlePeerList;
            }
        }

        /// <summary>
        /// When changing the Peer Info outside, other script should call this to set the appropriate status
        /// </summary>
        /// <param name="peersInfo"></param>
        /// <param name="newStatus"></param>
        public void SetSessionStatus(OrderedPeersInfo peersInfo, VideoCallStatus newStatus)
        {
            if ( peersInfo == null || allSessions == null ||
                 !allSessions.TryGetValue(peersInfo, out var session) ) return;
            if ( session == null ) return;
            session._status = newStatus;
            Debug.Log($"<color=green>\t{session}</color>");
        }

        
        //TODO better approach
        private void HandlePeerList(HashSet<OrderedPeersInfo> currentPeers)
        {
            if ( currentPeers == null ) return;
            var logPeers = string.Join(",", currentPeers);
            Debug.Log("<color=red>CurrentPeers</color>");
            Debug.Log($"<color=yellow>{logPeers}</color>");
            
            // the list is brand new
            if ( allSessions == null || allSessions.Count < 1 )
            {
                InitSession(currentPeers);
                return;
            }

            var currentWorkingPeers = allSessions
                .Where(e => e.Value._status == VideoCallStatus.Started || e.Value._status == VideoCallStatus.ShouldEnd)
                .Select(e => e.Value._peersInfo).ToHashSet();
            var endedPeers = allSessions
                .Where(e => e.Value._status == VideoCallStatus.Ended)
                .Select(e => e.Value._peersInfo).ToHashSet();

            var newPeers = currentPeers.Except(currentWorkingPeers);
            var shouldEndPeers = currentWorkingPeers.Except(currentPeers);
            Debug.Log("<color=yellow>3</color>");
            foreach (var newPeer in newPeers)
            {
                var videoCallSession = new VideoCallSession()
                {
                    _peersInfo = newPeer,
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


        private void InitSession(HashSet<OrderedPeersInfo> initSet)
        {
            allSessions ??= new Dictionary<OrderedPeersInfo, VideoCallSession>();
            foreach (var peersPeer in initSet)
            {
                allSessions.TryAdd(peersPeer, new VideoCallSession()
                {
                    _peersInfo = peersPeer,
                    _status = VideoCallStatus.ShouldStart
                });
            }

            OnShouldStartSession?.Invoke(allSessions.Values.ToList());
        }

        private void OnDisable()
        {
            if ( scanners == null ) return;
            foreach (var scanner in scanners)
            {
                if ( scanner != null ) scanner._onPeersChanged -= HandlePeerList;
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
        public OrderedPeersInfo _peersInfo;
        public VideoCallStatus _status;

        public override bool Equals(object obj)
        {
            if ( obj is VideoCallSession other )
            {
                return _peersInfo == other._peersInfo;
            }

            return false;
        }

        public override int GetHashCode()
        {
            if ( _peersInfo != null ) return _peersInfo.GetHashCode();
            return 0;
        }

        public override string ToString()
        {
            return _peersInfo + "(" + _status + ")";
        }

        public bool InvolveUser(int userId) => _peersInfo.HasUser(userId);
    }
}