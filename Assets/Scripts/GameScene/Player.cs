using Fusion;
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

        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfoSo;
        [SerializeField] private List<RuntimeAnimatorController> _animatorControllers;
        [SerializeField] private TMP_Text _textPlayerName;

        private ObjectPhase _phase = ObjectPhase.Init;

        [Networked, OnChangedRender(nameof(OnAvatarChanged))] public int _avatarIndex { get; set; } = -1;
        [Networked, OnChangedRender(nameof(OnNameChanged))] public string _playerName { get; set; } = "";

        public override void Spawned()
        {
            _controller.SetupComponents();
        }

        public override void Render()
        {
            switch (_phase)
            {
                case ObjectPhase.Init:
                    SetName(_playerInfoSo.PlayerName);
                    SetAvatar(_playerInfoSo.AvatarIndex);
                    _phase = ObjectPhase.Running;
                    break;
            }
        }

        public void SetName(string name)
        {
            _playerName = name;
        }

        public void SetAvatar(int index)
        {
            _avatarIndex = index;
        }

        private void OnAvatarChanged()
        {
            _animator.runtimeAnimatorController = _animatorControllers[_avatarIndex];
        }

        private void OnNameChanged()
        {
            _textPlayerName.text = _playerName;
        }
    }
}
