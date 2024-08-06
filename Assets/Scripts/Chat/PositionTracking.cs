using System;
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
            GatheringArea.OnPlayerEntered += HandlePlayerEnter;
            GatheringArea.OnPlayerExit += HandlePlayerExit;
        }

        private void OnDisable()
        {
            GatheringArea.OnPlayerEntered -= HandlePlayerEnter;
            GatheringArea.OnPlayerExit -= HandlePlayerExit;
        }

        private async void HandlePlayerEnter(int enteredUserId)
        {
            if ( enteredUserId != _userId ) return; // if I am not the one, entering the area
            _enablePositionTracking = false;
            await UniTask.DelayFrame(2);
            AEventHandler.ExecuteEvent(GlobalEvents.StopPositionTracking, _userId);
        }

        private void HandlePlayerExit(int exitedUserId)
        {
            if ( exitedUserId != _userId ) return; // if I am not the one, exiting the area
            _enablePositionTracking = true;
        }

        private async UniTask ReportPosition()
        {
            while (true)
            {
                await UniTask.WaitUntil(() => _userId > -1);
                await UniTask.WaitUntil(() => _enablePositionTracking);
                AEventHandler.ExecuteEvent(GlobalEvents.PlayerPositionUpdated, _userId,
                    new Vector2(transform.position.x, transform.position.y));
                await UniTask.WaitForSeconds(_reportInterval);
            }
        }
    }
}