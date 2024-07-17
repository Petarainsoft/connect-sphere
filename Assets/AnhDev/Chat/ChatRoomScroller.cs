using System;
using System.Collections.Generic;
using ConnectSphere;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace UI.Chat
{
    
    public class ChatRoomScroller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField]
        private List<ConnectSphere.Chat> _scrollerData;
        public EnhancedScroller _roomScroller;

        [SerializeField]
        public ChatRoomRowView _chatRoomRowViewPrefab;
        public event Action<int> OnChatSelected;

        public OnChatRoomRowViewClicked OnChatRoomSelected;

        private void Start()
        {
            _roomScroller.Delegate = this;
            _roomScroller.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            if ( _scrollerData == null )
            {
                return 0;
            }

            return _scrollerData.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 200f;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int
            dataIndex, int cellIndex)
        {
            ChatRoomRowView rowView = scroller.GetCellView(_chatRoomRowViewPrefab) as
                ChatRoomRowView;
            rowView.SetData(_scrollerData[dataIndex]);
            rowView._selected = OnChatRoomSelected;
            return rowView;
        }

        public void UpdateChatsData(PollDataResult pollDataResult)
        {
            _scrollerData = pollDataResult.data;
            _roomScroller.ReloadData();
        }
        

        public void OnCellSelected(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            Debug.Log($"Selected {_scrollerData[dataIndex].ToString()}");
        }
    }
}