using System.Collections.Generic;
using System.Linq;
using Chat;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class AreaChatRoomMonitor : MonoBehaviour
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

        private async void HandlePlayerEnter(int areaId, int userId, List<int> usersInArea)
        {
            Debug.Log($"<color=red>listPlayers {string.Join(",", usersInArea)}</color>");
            var iEnterArea = userId == _playerInfoSo.DatabaseId;
            var iEnterAreaWithSomeWereThere = iEnterArea && usersInArea.Count > 1;
            var iamInAreaAndOtherEnter = !iEnterArea && usersInArea.Contains(_playerInfoSo.DatabaseId);
            if (iEnterAreaWithSomeWereThere || iamInAreaAndOtherEnter)
            {
                foreach (var areaChatItem in _areaChatItems)
                {
                    var enableItem = (areaChatItem.AreaId == areaId);
                    areaChatItem.gameObject.SetActive(enableItem);
                    areaChatItem.enabled = enableItem;
                    if (areaChatItem.AreaId == areaId)
                    {
                        _vivoxHelper.JoinAreaChat(areaId);
                        _vivoxHelper.JoinAudio(areaId);
                        // _chatFrameTitle.text = $"{_playerInfoSo.RoomName} Office";
                    }
                }
            }
        }

        private void HandlePlayerExit(int areaId, int userId, List<int> usersInArea)
        {
            var iLeftArea = userId == _playerInfoSo.DatabaseId;
            var someoneLeftAndImLastOne = !iLeftArea && usersInArea.Count == 1 && usersInArea.Contains(_playerInfoSo.DatabaseId);
            if (iLeftArea || someoneLeftAndImLastOne)
            {
                foreach (var areaChatItem in _areaChatItems)
                {
                    areaChatItem.gameObject.SetActive(false);
                    areaChatItem.enabled = false;
                    if ( areaId == areaChatItem.AreaId)
                    {
                        _vivoxHelper.LeaveAreaChat(areaId);
                        _vivoxHelper.LeaveAudio(areaId);
                        // _chatFrameTitle.text = $"{_playerInfoSo.RoomName} Office";
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