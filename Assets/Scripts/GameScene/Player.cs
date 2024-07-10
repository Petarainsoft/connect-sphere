using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class Player : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController _controller;
        [SerializeField] private Animator _animator;

        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfoSo;
        [SerializeField] private List<RuntimeAnimatorController> _animatorControllers;
        [SerializeField] private TMP_Text _textPlayerName;

        [Networked, OnChangedRender(nameof(OnAvatarChanged))] public int _avatarIndex { get; set; } = -1;

        public override void Spawned()
        {
            //SetAvatar(_playerInfoSo.AvatarIndex);
            SetName(_playerInfoSo.PlayerName);
            _controller.SetupComponents();
        }

        public void SetName(string name)
        {
            _textPlayerName.text = name;
        }

        public void SetAvatar(int index)
        {
            _avatarIndex = index;
        }

        private void OnAvatarChanged()
        {
            _animator.runtimeAnimatorController = _animatorControllers[_avatarIndex];
            Debug.Log($"---------------- Set Avatar: {_avatarIndex}");
        }
    }
}
