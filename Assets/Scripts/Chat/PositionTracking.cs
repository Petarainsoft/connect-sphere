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
        private int userId = -1; // invalid userId
        private bool enablePositionTracking = true;

        [SerializeField] private float _reportInterval = 0.1f;

        private async void Start()
        {
            await UniTask.WaitUntil(() => GetComponent<Player>() != null);
            var player = GetComponent<Player>();
            await UniTask.WaitUntil(() => player != null && player.DatabaseId > -1);
            if ( player != null ) userId = player.DatabaseId;
            await ReportPosition();
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
            if ( enteredUserId != userId ) return; // if I am not the one, entering the area
            enablePositionTracking = false;
            await UniTask.DelayFrame(2);
            AEventHandler.ExecuteEvent(GlobalEvents.StopPositionTracking, userId);
        }

        private void HandlePlayerExit(int exitedUserId)
        {
            if ( exitedUserId != userId ) return; // if I am not the one, exiting the area
            enablePositionTracking = true;
        }

        private async UniTask ReportPosition()
        {
            while (true)
            {
                await UniTask.WaitUntil(() => userId > -1);
                await UniTask.WaitUntil(() => enablePositionTracking);
                AEventHandler.ExecuteEvent(GlobalEvents.PlayerPositionUpdated, userId,
                    new Vector2(transform.position.x, transform.position.y));
                await UniTask.WaitForSeconds(_reportInterval);
            }
        }
    }
}