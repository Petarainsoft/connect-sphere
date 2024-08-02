using System;
using System.Collections.Generic;
using AhnLab.EventSystem;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class VideoLayoutManager : MonoBehaviour
    {
        public LayoutMode _currentMode = LayoutMode.HorizontalTop;
        public LayoutMode _prevMode = LayoutMode.None;

        public UIToggle _horizontalToggle;
        public UIToggle _gridToggle;
        public UIToggle _splitToggle;

        public RectTransform _gridContainer;
        public RectTransform _horizontalContainer;
        public RectTransform _leftRightContainer;
        public RectTransform _focusImageContainer;
        
        #region Pooling
        public bool _collectionChecks = true;
        public int _maxPoolSize = 15;
        public int _defaultPoolSize = 5;
        public Transform _poolParent;
        public GameObject _remoteVideoPrefab;

        private IObjectPool<RemoteVideoContainer> mPool;

        private IObjectPool<RemoteVideoContainer> Pool
        {
            get
            {
                if ( mPool != null ) return mPool;
                mPool = new ObjectPool<RemoteVideoContainer>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                    OnDestroyPoolObject, _collectionChecks, _defaultPoolSize, _maxPoolSize);
                return mPool;
            }
        }

        private RemoteVideoContainer CreatePooledItem()
        {
            var connectionGameObject = Instantiate(_remoteVideoPrefab, Vector3.zero, Quaternion.identity, _poolParent);
            if ( connectionGameObject == null ) return null;
            var remoteVideoContainer = connectionGameObject.GetComponent<RemoteVideoContainer>();
            return remoteVideoContainer;
        }

        private void OnReturnedToPool(RemoteVideoContainer connection)
        {
            if ( connection == null ) return;
            connection.transform.SetParent(_poolParent);
            connection.gameObject.SetActive(false);
        }
      
        private void OnTakeFromPool(RemoteVideoContainer connection)
        {
            if ( connection != null )
            {
                connection.gameObject.SetActive(true);
            }
        }

        private void OnDestroyPoolObject(RemoteVideoContainer connection)
        {
            if ( connection != null ) Destroy(connection.gameObject);
        }
        
        #endregion
        
        public Dictionary<OrderedPeersInfo, RemoteVideoContainer> _peersInfos;

        public void OnEnable()
        {
            AEventHandler.RegisterEvent<OrderedPeersInfo, Texture>(GlobalEvents.OnReceivedRemoteVideo, HandleRemoteVideo);
            AEventHandler.RegisterEvent<OrderedPeersInfo>(GlobalEvents.OnCloseRemoteVideo, HandleCloseVideo);
        }

        public void OnDisable()
        {
            AEventHandler.UnregisterEvent<OrderedPeersInfo, Texture>(GlobalEvents.OnReceivedRemoteVideo, HandleRemoteVideo);
            AEventHandler.UnregisterEvent<OrderedPeersInfo>(GlobalEvents.OnCloseRemoteVideo, HandleCloseVideo);
        }

        private void HandleRemoteVideo(OrderedPeersInfo peersInfo, Texture videoTexture)
        {
            RectTransform targetContainer = null;
            switch (_currentMode)
            {
                case LayoutMode.None:
                    return;
                case LayoutMode.HorizontalTop:
                    targetContainer = _horizontalContainer;
                    break;
                case LayoutMode.CenterGrid:
                    targetContainer = _gridContainer;
                    break;
                case LayoutMode.Split:
                    targetContainer = _leftRightContainer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (peersInfo != null && _peersInfos != null && _peersInfos.TryGetValue(peersInfo, out var remoteVideoContainer))
            {
                if ( remoteVideoContainer == null ) return;
                remoteVideoContainer.transform.SetParent(targetContainer);
                remoteVideoContainer.SetRemoteVideoTexture(videoTexture);
            }
            else
            {
                remoteVideoContainer = Pool.Get();
                if ( remoteVideoContainer == null ) return;
                remoteVideoContainer.SetRemoteVideoTexture(videoTexture);
                remoteVideoContainer.transform.SetParent(targetContainer);
                _peersInfos?.Add(peersInfo, remoteVideoContainer);
            }
        }
        
        private void HandleCloseVideo(OrderedPeersInfo peersInfo)
        {
            if (peersInfo != null && _peersInfos != null && _peersInfos.TryGetValue(peersInfo, out var remoteVideoContainer))
            {
                if ( remoteVideoContainer == null ) return;
                _peersInfos.Remove(peersInfo);
                Pool.Release(remoteVideoContainer);
            }
        }


        private void Start()
        {
            _peersInfos = new Dictionary<OrderedPeersInfo, RemoteVideoContainer>();
            _horizontalToggle.Toggle.onValueChanged.AddListener((isOn) =>
            {
                if ( isOn ) _currentMode = LayoutMode.HorizontalTop;
            });
            _gridToggle.Toggle.onValueChanged.AddListener((isOn) =>
            {
                if ( isOn ) _currentMode = LayoutMode.CenterGrid;
            });
            _splitToggle.Toggle.onValueChanged.AddListener((isOn) =>
            {
                if ( isOn ) _currentMode = LayoutMode.Split;
            });
        }

        private void Update()
        {
            if ( _currentMode == _prevMode ) return;
            ChangeLayout(_prevMode, _currentMode);
            _prevMode = _currentMode;
        }

        private void ChangeLayout(LayoutMode prevMode, LayoutMode currentMode)
        {
            RectTransform sourceContainer = null;
            switch (prevMode)
            {
                case LayoutMode.None:
                    return;
                case LayoutMode.HorizontalTop:
                    sourceContainer = _horizontalContainer;
                    break;
                case LayoutMode.CenterGrid:
                    sourceContainer = _gridContainer;
                    break;
                case LayoutMode.Split:
                    sourceContainer = _leftRightContainer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(prevMode), prevMode, null);
            }

            if ( sourceContainer == null ) return;
            RectTransform targetContainer = null;
            switch (currentMode)
            {
                case LayoutMode.None:
                    break;
                case LayoutMode.HorizontalTop:
                    targetContainer = _horizontalContainer;
                    break;
                case LayoutMode.CenterGrid:
                    targetContainer = _gridContainer;
                    break;
                case LayoutMode.Split:
                    targetContainer = _leftRightContainer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentMode), currentMode, null);
            }

            var childCount = sourceContainer.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var child = sourceContainer.GetChild(0);

                // first video comes to the right side
                if ( currentMode == LayoutMode.Split && i == 0 )
                {
                    child.SetParent(_focusImageContainer);
                }
                else
                {
                    if ( child != null ) child.SetParent(targetContainer);
                }

                if ( currentMode == LayoutMode.HorizontalTop )
                {
                    var ap = child.GetComponent<AspectRatioFitter>();
                    if ( ap != null )
                    {
                        ap.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
                    }
                } else if ( currentMode == LayoutMode.Split )
                {
                    var ap = child.GetComponent<AspectRatioFitter>();
                    if ( ap != null )
                    {
                        ap.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
                    }
                }
            }

            if ( currentMode != LayoutMode.Split && _focusImageContainer.childCount == 1 )
            {
                _focusImageContainer.GetChild(0).SetParent(targetContainer);
            }

            // set target view as second last sibling
            targetContainer.parent.parent.SetSiblingIndex(transform.childCount - 2);
        }
    }

    public enum LayoutMode
    {
        None,
        HorizontalTop,
        CenterGrid,
        Split
    }
}