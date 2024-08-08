using System;
using System.Collections.Generic;
using AhnLab.EventSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ConnectSphere
{
    /// <summary>
    /// Attach to player, reporting player position.
    /// Listen to the GatheringArea event to start/stop reporting position.
    /// When player enters an area, it will auto connect to other peers, no need to report its position.
    /// When player leaves, keep reporting position.
    /// </summary>
    public class PositionTracking : MonoBehaviour
    {
        private int _userId = -1; // invalid userId
        private bool _enablePositionTracking = true;

        [SerializeField] private float _reportInterval = 0.1f;

        public int UserId => _userId;

        private async void Start()
        {
            await UniTask.WaitUntil(() => GetComponent<Player>() != null);
            var player = GetComponent<Player>();
            await UniTask.WaitUntil(() => player != null && player.DatabaseId > -1);
            if ( player != null ) _userId = player.DatabaseId;
            try
            {
                await ReportPosition();
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning("ReportPosition has been cancelled");
            }
        }

        private void OnEnable()
        {
            GatheringArea.OnPlayerEnteredArea += HandlePlayerEnter;
            GatheringArea.OnPlayerExitArea += HandlePlayerExit;
        }

        private void OnDisable()
        {
            GatheringArea.OnPlayerEnteredArea -= HandlePlayerEnter;
            GatheringArea.OnPlayerExitArea -= HandlePlayerExit;
        }

        private void OnDestroy()
        {
            if (_userId > -1)
            {
                AEventHandler.ExecuteEvent(GlobalEvents.StopPositionTracking, _userId);
            }
        }

        private void HandlePlayerEnter(int areaId, int enteredUserId, List<int> userInArea)
        {
            if ( enteredUserId == _userId ) // me entering
            {
                _enablePositionTracking = false;
                Debug.Log($"STOP TRACKING POSITION FOR USER {_userId}");
                AEventHandler.ExecuteEvent(GlobalEvents.StopPositionTracking, _userId);
                return;
            }
            
            if ( userInArea.Contains(_userId) ) // I am in the area
            {
                _enablePositionTracking = false;
                Debug.Log($"STOP TRACKING POSITION FOR USER {_userId}");
                AEventHandler.ExecuteEvent(GlobalEvents.StopPositionTracking, _userId);
            }
        }
   
        private void HandlePlayerExit(int areaId, int exitUserId, List<int> userInArea)
        {
            if ( exitUserId == _userId ) // me exiting
            {
                _enablePositionTracking = true;
                Debug.Log($"RESUME TRACKING POSITION FOR USER {_userId}");
                return;
            }
            
            if ( userInArea.Contains(_userId) ) // I am in the area
            {
                _enablePositionTracking = false;
                Debug.Log($"STOP TRACKING POSITION FOR USER {_userId}");
                AEventHandler.ExecuteEvent(GlobalEvents.StopPositionTracking, _userId);
            }
        }

        private async UniTask ReportPosition()
        {
            while (true)
            {
                await UniTask.WaitUntil(() => _userId > -1);
                await UniTask.WaitUntil(() => _enablePositionTracking);
                if (this != null && gameObject != null && transform != null)
                {
                    AEventHandler.ExecuteEvent(GlobalEvents.PlayerPositionUpdated, _userId,
                        new Vector2(transform.position.x, transform.position.y));
                }
                await UniTask.WaitForSeconds(_reportInterval);
            }
        }
    }
}