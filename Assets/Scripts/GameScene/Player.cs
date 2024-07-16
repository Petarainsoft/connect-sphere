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

        [Networked] public int AvatarIndex { get; set; } = -1;
        [HideInInspector] [Networked] public NetworkString<_16> NickName { get; private set; }

        public static Action<int> OnEmoticonClicked;

        private void OnEnable()
        {
            OnEmoticonClicked += ShowBubbleChat;
        }

        private void OnDisable()
        {
            OnEmoticonClicked -= ShowBubbleChat;
        }

        public override void Spawned()
        {
            _controller.SetupComponents();
            if (Object.HasStateAuthority)
            {
                NickName = _playerInfoSo.PlayerName;
                AvatarIndex = _playerInfoSo.AvatarIndex;
            }
            _textPlayerName.text = $"{NickName}";
            _animator.runtimeAnimatorController = _animatorControllers[AvatarIndex];
        }

        private void ShowBubbleChat(int spriteIndex)
        {
            _bubbleChat.SetBubbleSprite(spriteIndex);
        }
    }
}
