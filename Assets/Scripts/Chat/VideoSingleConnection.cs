using System;
using ConnectSphere;
using Unity.RenderStreaming;
using UnityEngine;

namespace Chat
{
    public class VideoSingleConnection : MonoBehaviour
    {
        [SerializeField] public VideoStreamSender _webCamStreamer;
        [SerializeField] public VideoStreamReceiver _receiveVideoViewer;
        [SerializeField] public SingleConnection _singleWebRtcConnection;

        private OrderedPeersInfo myPeersInfo;

        public void SetCameraStreamerSource(Texture sourceTexture)
        {
            _webCamStreamer.sourceTexture = sourceTexture;
        }

        public void SetSenderCodec(VideoCodecInfo sendCodec)
        {
            _webCamStreamer.SetCodec(sendCodec);
        }

        public void SetReceiveCodec(VideoCodecInfo receiveCodec)
        {
            _receiveVideoViewer.SetCodec(receiveCodec);
        }

        public void SetCameraStreamerSize(uint width, uint height)
        {
            _webCamStreamer.width = width;
            _webCamStreamer.height = height;
        }

        public void SetOrderedPeersInfo(OrderedPeersInfo peersInfoInfo)
        {
            myPeersInfo = peersInfoInfo;
        }
        
        public void RegisterReceivedTexture(Action<Texture> OnReceiveVideoTexture)
        {
            if ( _receiveVideoViewer != null )
                _receiveVideoViewer.OnUpdateReceiveTexture = (receivetexture) =>
                {
                    OnReceiveVideoTexture?.Invoke(receivetexture);
                };
        }

        private void Release()
        {
            if ( _receiveVideoViewer != null ) _receiveVideoViewer.OnUpdateReceiveTexture = null;
            myPeersInfo = null;
            GetComponent<ReturnToPool>()?.Return();
        }

        public void CreateConnection(string connectionUniqueId)
        {
            _singleWebRtcConnection.CreateConnection(connectionUniqueId);
        }

        public void DeleteConnection(string peersConnectionId)
        {
            _singleWebRtcConnection.DeleteConnection(peersConnectionId);
            Release();
        }
    }
}