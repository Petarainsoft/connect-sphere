using System;
using System.Linq;
using Unity.RenderStreaming;
using UnityEngine;

namespace Chat
{
    public enum SignalingType
    {
        WebSocket,
        Http,
    }
    
    [Serializable]
    public class RenderStreamingSettings
    {
        public const int DefaultStreamWidth = 1280;
        public const int DefaultStreamHeight = 720;

        [SerializeField]
        private bool useDefaultSettings = true;
        [SerializeField]
        private SignalingType signalingType = SignalingType.WebSocket;
        [SerializeField]
        private string signalingAddress = "localhost";
        [SerializeField]
        private int signalingInterval = 5000;
        [SerializeField]
        private bool signalingSecured = false;
        [SerializeField]
        private Vector2Int streamSize = new Vector2Int(DefaultStreamWidth, DefaultStreamHeight);
        [SerializeField]
        private VideoCodecInfo receiverVideoCodec = null;
        [SerializeField]
        private VideoCodecInfo senderVideoCodec = null;
        
        public void ApplyH264Codec()
        {
            var codecArray = VideoStreamReceiver.GetAvailableCodecs();
            var codec = Array.Find<VideoCodecInfo>(VideoStreamReceiver.GetAvailableCodecs().ToArray(), codec => codec is H264CodecInfo);
            if ( senderVideoCodec == null )
            {
                senderVideoCodec = codec;
            }
            
            if ( receiverVideoCodec == null )
            {
                receiverVideoCodec = codec;
            }
        }


        public bool UseDefaultSettings
        {
            get { return useDefaultSettings; }
            set { useDefaultSettings = value; }
        }

        public SignalingType SignalingType
        {
            get { return signalingType; }
            set { signalingType = value; }
        }

        public string SignalingAddress
        {
            get { return signalingAddress; }
            set { signalingAddress = value; }
        }

        public bool SignalingSecured
        {
            get { return signalingSecured; }
            set { signalingSecured = value; }
        }

        public int SignalingInterval
        {
            get { return signalingInterval; }
            set { signalingInterval = value; }
        }

        public SignalingSettings SignalingSettings
        {
            get
            {
                switch (signalingType)
                {
                    case SignalingType.WebSocket:
                    {
                        var schema = signalingSecured ? "wss" : "ws";
                        return new WebSocketSignalingSettings
                        (
                            url: $"{schema}://{signalingAddress}"
                        );
                    }
                    case SignalingType.Http:
                    {
                        var schema = signalingSecured ? "https" : "http";
                        return new HttpSignalingSettings
                        (
                            url: $"{schema}://{signalingAddress}",
                            interval: signalingInterval
                        );
                    }
                }

                throw new InvalidOperationException();
            }
        }

        public Vector2Int StreamSize
        {
            get { return streamSize; }
            set { streamSize = value; }
        }

        public VideoCodecInfo ReceiverVideoCodec
        {
            get { return receiverVideoCodec; }
            set { receiverVideoCodec = value; }
        }

        public VideoCodecInfo SenderVideoCodec
        {
            get { return senderVideoCodec; }
            set { senderVideoCodec = value; }
        }
    }
}