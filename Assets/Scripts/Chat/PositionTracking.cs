using System;
using System.Collections;
using System.Collections.Generic;
using AhnLab.EventSystem;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class PositionTracking : MonoBehaviour
    {
        [Tooltip("Speed at which the object moves.")] [SerializeField]
        private float moveSpeed = 10f;

        [Tooltip("Time interval to change direction.")] [SerializeField]
        private float changeDirectionInterval = 2.0f;

        private int userId = -1;
        private Vector2 moveDirection;
        private float timeSinceLastChange;
        //
        // public void SetUserId(int newUserId)
        // {
        //     userId = newUserId;
        //     GetComponentInChildren<TextMeshProUGUI>().text = userId.ToString();
        // }
        
        private void Awake()
        {
            var player = GetComponent<Player>();
            if ( player != null )
            {
                userId = player.DatabaseId;
            }
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

            if ( userId > -1 )
            {
                AEventHandler.ExecuteEvent(GlobalEvents.PositionUpdated, userId,
                    new Vector2(transform.position.x, transform.position.y));
                
            }
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