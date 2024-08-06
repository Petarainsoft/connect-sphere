using System;
using System.Collections;
using System.Collections.Generic;
using Chat;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class AreaChatItem : MonoBehaviour
    {
        [SerializeField] private UIButton _openChat;
        [SerializeField] private TMP_Text _areaNameText;
        [SerializeField] private VivoxServiceHelper _vivoxHelper;

        [SerializeField]
        private int areaId;

        public int AreaId => areaId;

        private void Start()
        {
            _areaNameText.text = $"Area {areaId + 1}";
        }

        private void OnEnable()
        {
            _openChat.OnClick.OnTrigger.Event.AddListener(JoinChatArea);
        }
        
        private void OnDisable()
        {
            _openChat.OnClick.OnTrigger.Event.RemoveListener(JoinChatArea);
        }

        private void JoinChatArea() =>   _vivoxHelper.JoinAreaChat(areaId);
    }
}
