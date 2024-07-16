using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class Player : NetworkBehaviour
    {
        enum ObjectPhase
        {
            Init = 0, Running = 1
        }

        [Header("References")]
        [SerializeField] private PlayerController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private BubbleChat _bubbleChat;
        [SerializeField] private TMP_Text _textPlayerName;

        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfoSo;
        [SerializeField] private List<RuntimeAnimatorController> _animatorControllers;

        private ObjectPhase _phase = ObjectPhase.Init;

        [Networked, OnChangedRender(nameof(OnAvatarChanged))] public int _avatarIndex { get; set; } = -1;
        [HideInInspector] [Networked] public NetworkString<_16> NickName { get; private set; }

        public static Action<int> OnEmoticonClicked;

        private void OnEnable()
        {
            OnEmoticonClicked += ShowBubbleChatRpc;
        }

        private void OnDisable()
        {
            OnEmoticonClicked -= ShowBubbleChatRpc;
        }

        public override void Spawned()
        {
            _controller.SetupComponents();
            if (Object.HasStateAuthority)
            {
                NickName = _playerInfoSo.PlayerName;
            }
            _textPlayerName.text = $"{NickName}";
        }

        public override void Render()
        {
            switch (_phase)
            {
                case ObjectPhase.Init:
                    SetAvatar(_playerInfoSo.AvatarIndex);
                    _phase = ObjectPhase.Running;
                    break;
            }
        }

        public void SetAvatar(int index)
        {
            _avatarIndex = index;
        }

        private void OnAvatarChanged()
        {
            _animator.runtimeAnimatorController = _animatorControllers[_avatarIndex];
        }

        [Rpc()]
        private void ShowBubbleChatRpc(int spriteIndex)
        {
            StartCoroutine(_bubbleChat.Show(spriteIndex));
        }
    }
}
