using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class RemoteVideoContainer : MonoBehaviour
    {
        public RawImage _remoteVideoImage;

        public void SetRemoteVideoTexture(Texture remoteTexture)
        {
            _remoteVideoImage.texture = remoteTexture;
        }
    }
}
