using System;
using Unity.RenderStreaming;
using UnityEngine;

namespace Chat
{
    public class VideoSingleConnection : MonoBehaviour
    {
        [SerializeField] public VideoStreamSender webCamStreamer;
        [SerializeField] public VideoStreamReceiver receiveVideoViewer;
        [SerializeField] public SingleConnection singleConnection;
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

        private void StopStream(string connectionid)
        {
            if ( singleConnection.ExistConnection(connectionid) )
            {
                singleConnection.DeleteConnection(connectionid);
                ConnectionID = string.Empty;
            }
        }

        public void SetTextureIndex(int index)
        {
            _index = index;
            if ( receiveVideoViewer != null ) receiveVideoViewer.OnUpdateReceiveTexture += OnUpdateReceiveTexture;
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
            if ( receiveVideoViewer != null ) receiveVideoViewer.OnUpdateReceiveTexture -= OnUpdateReceiveTexture;
        }
    }
}