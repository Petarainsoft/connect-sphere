using System.Linq;
using AccountManagement;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Chat
{
    public delegate void OnChatRoomRowViewClicked(ConnectSphere.Chat chatRoomData);

    public class ChatRoomRowView : EnhancedScrollerCellView
    {
        [SerializeField] private TextMeshProUGUI _roomName;
        [SerializeField] private TextMeshProUGUI _roomLastMessage;
        [SerializeField] private Image _roomIcon;

        public OnChatRoomRowViewClicked _selected;

        [SerializeField] private ConnectSphere.Chat _chatRoomData;

        public void SetData(ConnectSphere.Chat chatRoomData)
        {
            // _roomName.text = chatRoomData.name;
            _chatRoomData = chatRoomData;
            _roomLastMessage.text = chatRoomData.last_message != null ? chatRoomData.last_message.content : "";
            // var userData = chatRoomData.users.FirstOrDefault(u => u.id != ApiManager.Inst.UserId);
            // _roomName.text = userData != null ? userData.username : "";
        }

        public void Select()
        {
            if ( _selected != null ) _selected(_chatRoomData);
        }
    }
}