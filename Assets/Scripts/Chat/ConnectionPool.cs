using Chat;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace ConnectSphere
{
    /// <summary>
    /// The pool for webrtc video connection among peers
    /// </summary>
    public class ConnectionPool : MonoBehaviour
    {
        public bool _collectionChecks = true;
        public int _maxPoolSize = 15;
        public int _defaultPoolSize = 5;

        public GameObject _videoConnectPrefab;

        private IObjectPool<VideoSingleConnection> mPool;

        public IObjectPool<VideoSingleConnection> Pool
        {
            get
            {
                if ( mPool != null ) return mPool;
                mPool = new ObjectPool<VideoSingleConnection>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                    OnDestroyPoolObject, _collectionChecks, _defaultPoolSize, _maxPoolSize);
                return mPool;
            }
        }

        private VideoSingleConnection CreatePooledItem()
        {
            var connectionGameObject = Instantiate(_videoConnectPrefab, Vector3.zero, quaternion.identity, transform);
            if ( connectionGameObject == null ) return null;
            var videoSingleConnection = connectionGameObject.AddComponent<VideoSingleConnection>();
            var returnToPool = connectionGameObject.AddComponent<ReturnToPool>();
            if ( returnToPool != null ) returnToPool.pool = Pool;
            return videoSingleConnection;
        }

        private void OnReturnedToPool(VideoSingleConnection connection)
        {
            if ( connection == null ) return;
            connection.Release();
            connection.gameObject.SetActive(false);
        }
      
        private void OnTakeFromPool(VideoSingleConnection connection)
        {
            if ( connection != null ) connection.gameObject.SetActive(true);
        }

        private void OnDestroyPoolObject(VideoSingleConnection connection)
        {
            if ( connection != null ) Destroy(connection.gameObject);
        }
    }
}