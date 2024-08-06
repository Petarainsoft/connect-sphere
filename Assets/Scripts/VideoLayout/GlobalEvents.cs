using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class GlobalEvents
    {
        public const string OnReceivedRemoteVideo = "ON_RECEIVED_REMOTE";
        public const string OnCloseRemoteCamera = "ON_CLOSE_REMOTE";
        public const string DisplayLocalVideo = "DISPLAY_LOCAL_VIDEO";
        public const string CloseLocalVideo = "CLOSE_LOCAL_VIDEO";
        public const string PlayerPositionUpdated = "POSITION_UPDATED";
        public const string StopPositionTracking = "STOP_POSITION_TRACKING";
    }
}
