using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chat;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class MonitorAreaChat : MonoBehaviour
    {
        [Header("Gather Areas Handle Connection")] [SerializeField]
        private List<GatheringArea> _areas;

        [SerializeField] private List<AreaChatItem> _areaChatItems;


        [SerializeField] private VivoxServiceHelper _vivoxHelper;

        [SerializeField] private TMP_Text _chatFrameTitle;

        [SerializeField] private PlayerInfoSO _playerInfoSo;


        private void Awake()
        {
            foreach (var areaItem in _areaChatItems)
            {
                areaItem.gameObject.SetActive(false);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            GatheringArea.OnPlayerEnteredArea += HandlePlayerEnter;
            GatheringArea.OnPlayerExitArea += HandlePlayerExit;
        }

        private void HandlePlayerExit(int areaId)
        {
            Debug.Log($"<color=red>PlayerEXIT {areaId}</color>");
            var area = _areas.FirstOrDefault(e => e.AreaId == areaId);
            if ( area == null ) return;
            var listPlayers = area.PlayersInArea;
            Debug.Log($"<color=red>listPlayers {string.Join(",", listPlayers)}</color>");
            // me went out
            var myId = PlayerPrefs.GetInt("userId");
            Debug.Log($"<color=red>MyId {myId}</color>");
            // me not in the area just raise leave-event
            if ( !listPlayers.Any(p => p.GetComponent<Player>().DatabaseId == myId) )
            {
                foreach (var areaChatItem in _areaChatItems)
                {
                    if ( areaChatItem.AreaId == areaId )
                    {
                        areaChatItem.gameObject.SetActive(false);
                        areaChatItem.enabled = false;
                        _vivoxHelper.LeaveAreaChat(areaId); // me leave that area}
                        _vivoxHelper.LeaveAudio(areaId);
                        _chatFrameTitle.text = $"{_playerInfoSo.RoomName} Office";
                    }
                }
            }
        }

        private async void HandlePlayerEnter(int areaId)
        {
            Debug.Log($"<color=red>PlayerENTER {areaId}</color>");
            var area = _areas.FirstOrDefault(e => e.AreaId == areaId);
            if ( area == null ) return;
            var listPlayers = area.PlayersInArea;
            Debug.Log(
                $"<color=red>listPlayers {string.Join(",", listPlayers.Select(e => e.GetComponent<Player>().DatabaseId))}</color>");
            // me went in
            var myId = PlayerPrefs.GetInt("userId");
            Debug.Log($"<color=red>MyId {myId}</color>");
            if ( listPlayers.Any(p => p.GetComponent<Player>().DatabaseId == myId) )
            {
                foreach (var areaChatItem in _areaChatItems)
                {
                    areaChatItem.gameObject.SetActive((areaChatItem.AreaId == areaId));
                    areaChatItem.enabled = areaChatItem.AreaId == areaId;
                    if ( areaChatItem.AreaId == areaId )
                    {
                        _chatFrameTitle.text = $"Area {areaId + 1}";
                    }
                }
            }
        }

        private void OnDestroy()
        {
            GatheringArea.OnPlayerEnteredArea -= HandlePlayerEnter;
            GatheringArea.OnPlayerExitArea -= HandlePlayerExit;
        }
    }
}