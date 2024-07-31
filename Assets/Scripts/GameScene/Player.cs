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
        [SerializeField] private TMP_Text _textTitle;

        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfoSo;
        [SerializeField] private List<RuntimeAnimatorController> _animatorControllers;

        [Header("Events")]
        [SerializeField] private IntegerEventHandlerSO _onEmoticonClicked;
        [SerializeField] private DoubleStringEventHandlerSO _onLongNameChanged;
        [SerializeField] private IntegerEventHandlerSO _onAvatarChanged;

        [Networked, OnChangedRender(nameof(OnNickNameChanged))] public NetworkString<_16> NickName { get; private set; }
        [Networked, OnChangedRender(nameof(OnTitleTextChanged))] public NetworkString<_16> Title { get; private set; }
        [Networked, OnChangedRender(nameof(OnAvatarIndexChanged))] public int AvatarIndex { get; set; } = -1;
        [Networked] public NetworkString<_16> Email { get; private set; }
        [Networked] public int DatabaseId { get; private set; } = -1;

        private void OnEnable()
        {
            _onEmoticonClicked.OnEventRaised += ShowBubbleChat;
            _onLongNameChanged.OnEventRaised += SetLongName;
            _onAvatarChanged.OnEventRaised += SetAnimator;
        }

        private void OnDisable()
        {
            _onEmoticonClicked.OnEventRaised -= ShowBubbleChat;
            _onLongNameChanged.OnEventRaised -= SetLongName;
            _onAvatarChanged.OnEventRaised -= SetAnimator;
        }

        public override void Spawned()
        {
            SetDBIdAndEmail();
            _controller.SetupComponents();
            if (Object.HasStateAuthority)
            {
                NickName = _playerInfoSo.PlayerName;
                Title = _playerInfoSo.Title;
                AvatarIndex = _playerInfoSo.AvatarIndex;
                Email = _playerInfoSo.Email;
                DatabaseId = _playerInfoSo.DatabaseId;
            }
            FindObjectOfType<GameManager>().TrackNewPlayer(this);
            _textPlayerName.text = $"{NickName}";
            _textTitle.text = $"{Title}";
            _animator.runtimeAnimatorController = _animatorControllers[AvatarIndex];
        }

        private void ShowBubbleChat(int spriteIndex)
        {
            _bubbleChat.SetBubbleSprite(spriteIndex);
        }

        private void SetDBIdAndEmail()
        {
            _playerInfoSo.Email = PlayerPrefs.GetString("username");
            _playerInfoSo.DatabaseId = PlayerPrefs.GetInt("userId");
        }

        private void SetNickName(string text)
        {
            if (Object.HasStateAuthority)
            {
                NickName = text;
            }
        }

        private void OnNickNameChanged()
        {
            _textPlayerName.text = $"{NickName}";
        }

        private void SetLongName(string textName, string textTitle)
        {
            if (Object.HasStateAuthority)
            {
                if (!textName.Equals(string.Empty))
                    NickName = textName;
                Title = textTitle;
            }
        }

        private void OnTitleTextChanged()
        {
            _textTitle.text = $"{Title}";
        }

        private void SetAnimator(int index)
        {
            if (Object.HasStateAuthority)
            {
                AvatarIndex = index;
            }
        }

        private void OnAvatarIndexChanged()
        {
            _animator.runtimeAnimatorController = _animatorControllers[AvatarIndex];
        }
    }
}
