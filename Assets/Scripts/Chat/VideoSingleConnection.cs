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

        public void SetOrderedPeersInfo(OrderedPeersInfo peersInfo)
        {
            myPeersInfo = peersInfo;
        }

        public void SetCameraStreamerSource(Texture sourceTexture)
        {
            if ( _webCamStreamer != null ) _webCamStreamer.sourceTexture = sourceTexture;
        }

        public void SetSenderCodec(VideoCodecInfo sendCodec)
        {
            if ( _webCamStreamer != null ) _webCamStreamer.SetCodec(sendCodec);
        }

        public void SetReceiveCodec(VideoCodecInfo receiveCodec)
        {
            if ( _receiveVideoViewer != null ) _receiveVideoViewer.SetCodec(receiveCodec);
        }

        public void SetCameraStreamerSize(uint width, uint height)
        {
            if ( _webCamStreamer == null ) return;
            _webCamStreamer.width = width;
            _webCamStreamer.height = height;
        }

        public void RegisterReceivedTexture(Action<OrderedPeersInfo, Texture> OnReceiveVideoTexture)
        {
            if ( _receiveVideoViewer == null ) return;
            _receiveVideoViewer.OnUpdateReceiveTexture = receivedTexture =>
                OnReceiveVideoTexture?.Invoke(myPeersInfo, receivedTexture);
        }

        private void Release()
        {
            if ( _receiveVideoViewer != null ) _receiveVideoViewer.OnUpdateReceiveTexture = null;
            myPeersInfo = null;
            GetComponent<ReturnToPool>()?.Return();
        }

        public void CreateConnection(string connectionUniqueId)
        {
            if ( _singleWebRtcConnection != null ) _singleWebRtcConnection.CreateConnection(connectionUniqueId);
        }

        public void DeleteConnection(string peersConnectionId)
        {
            if ( _singleWebRtcConnection != null ) _singleWebRtcConnection.DeleteConnection(peersConnectionId);
            Release();
        }
    }
}