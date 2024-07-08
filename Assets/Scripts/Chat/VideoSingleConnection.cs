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
        public bool IsWorking = false;
        
        private int index = 0;
        private Action<Texture, int> cb;

        public void SetIndex(int Index)
        {
            index = Index;
            receiveVideoViewer.OnUpdateReceiveTexture += texture =>
                {
                    cb?.Invoke(texture, index);
                }
                ;
        }

        public void SetTextureReceiveCb(Action<Texture, int> onTextureReceive)
        {
            cb = onTextureReceive;
        }
    }
}