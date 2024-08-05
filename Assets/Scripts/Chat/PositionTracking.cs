using AhnLab.EventSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ConnectSphere
{
    public class PositionTracking : MonoBehaviour
    {
        [Tooltip("Speed at which the object moves.")] [SerializeField]
        private float _moveSpeed = 10f;

        [Tooltip("Time interval to change direction.")] [SerializeField]
        private float _changeDirectionInterval = 2.0f;

        private int userId = -1;
        private Vector2 moveDirection;

        private float timeSinceLastChange;

        private bool enablePositionTracking = false;

        // public void SetUserId(int newUserId)
        // {
        //     userId = newUserId;
        //     GetComponentInChildren<TextMeshProUGUI>().text = userId.ToString();
        // }

        private void Awake()
        {
            GatheringArea.OnPlayerEntered += HandlePlayerEnter;
        }

        private void OnDisable()
        {
            GatheringArea.OnPlayerExit -= HandlePlayerExit;
        }

        private void HandlePlayerEnter(int playerId)
        {
            enablePositionTracking = false;
            AEventHandler.ExecuteEvent(GlobalEvents.StopPositionTracking, userId);
        }
        
        private void HandlePlayerExit(int playerId)
        {
            enablePositionTracking = true;
        }


        private async void Start()
        {
            await UniTask.WaitUntil(() => GetComponent<Player>() != null);
            var player = GetComponent<Player>();
            await UniTask.WaitUntil(() => player.DatabaseId > -1);
            userId = player.DatabaseId;
        }

        // private void Start()
        // {
        //     ChangeDirection();
        // }

        private void Update()
        {
            // MoveObject();
            // timeSinceLastChange += Time.deltaTime;
            // if ( timeSinceLastChange >= changeDirectionInterval )
            // {
            //     ChangeDirection();
            //     timeSinceLastChange = 0f;
            // }

            if ( userId <= -1 ) return;
            if ( !enablePositionTracking ) return;
            AEventHandler.ExecuteEvent(GlobalEvents.PositionUpdated, userId,
                new Vector2(transform.position.x, transform.position.y));
        }

        // private void MoveObject()
        // {
        //     transform.Translate(moveDirection*moveSpeed*Time.deltaTime);
        // }
        //
        // private void ChangeDirection()
        // {
        //     float randomAngle = UnityEngine.Random.Range(0f, 360f);
        //     moveDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        // }
    }
}