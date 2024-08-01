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
        public string ConnectionID;
        public bool IsWorking = false;

        private int _index = -1;
        private Action<Texture, int> cb;

        // private void OnEnable()
        // {
        //     receiveVideoViewer.OnStoppedStream += StopStream;
        // }
        //
        // private void OnDisable()
        // {
        //     receiveVideoViewer.OnStoppedStream -= StopStream;
        // }

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

        private void StopStream(string connectionid)
        {
            if ( _singleWebRtcConnection.ExistConnection(connectionid) )
            {
                _singleWebRtcConnection.DeleteConnection(connectionid);
                ConnectionID = string.Empty;
            }
        }

        public void SetTextureIndex(int index)
        {
            _index = index;
            if ( _receiveVideoViewer != null ) _receiveVideoViewer.OnUpdateReceiveTexture += OnUpdateReceiveTexture;
        }

        private void OnUpdateReceiveTexture(Texture texture)
        {
            if ( _index == -1 )
            {
                Debug.Log("Invalid texture index");
                return;
            }

            cb?.Invoke(texture, _index);
        }

        public void SetTextureReceiveCb(Action<Texture, int> onTextureReceive)
        {
            cb = onTextureReceive;
        }

        public void Release()
        {
            cb = null;
            if ( _receiveVideoViewer != null ) _receiveVideoViewer.OnUpdateReceiveTexture -= OnUpdateReceiveTexture;
            GetComponent<ReturnToPool>().Return();
        }

        public void CreateConnection(string connectionUniqueId)
        {
            _singleWebRtcConnection.CreateConnection(connectionUniqueId);
            IsWorking = true;
        }

        public void DeleteConnection(string peersConnectionId)
        {
            _singleWebRtcConnection.DeleteConnection(peersConnectionId);
            IsWorking = false;
            ConnectionID = string.Empty;
            Release();
        }
    }
}