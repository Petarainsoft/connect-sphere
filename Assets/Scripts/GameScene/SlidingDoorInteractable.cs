using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fusion;

namespace ConnectSphere
{
    public class SlidingDoorInteractable : Interactable
    {
        [SerializeField] private Transform _leftDoor;
        [SerializeField] private Transform _rightDoor;
        [SerializeField] private bool _verticalDirection;
        [HideInInspector] [Networked] public bool IsActivated { get; set; }

        private float _initialLeft;
        private float _initialRight;
        private float _moveDuration = 0.4f;

        private void Awake()
        {
            if (!_verticalDirection)
            {
                _initialLeft = _leftDoor.localPosition.x;
                _initialRight = _rightDoor.localPosition.x;
            }
            else
            {
                _initialLeft = _leftDoor.localPosition.y;
                _initialRight = _rightDoor.localPosition.y;
            }
        }

        public override void Spawned()
        {
            SetStartingPosition();
        }

        private void SetStartingPosition()
        {
            if (!_verticalDirection)
            {
                if (IsActivated)
                {
                    _leftDoor.DOLocalMoveX(-3.275f, 0);
                    _rightDoor.DOLocalMoveX(1.525f, 0);
                }
                else
                {
                    _leftDoor.DOLocalMoveX(_initialLeft, 0);
                    _rightDoor.DOLocalMoveX(_initialRight, 0);
                }
            }
            else
            {
                if (IsActivated)
                {
                    _leftDoor.DOLocalMoveY(2.815f, 0);
                    _rightDoor.DOLocalMoveY(-1.31f, 0);
                }
                else
                {
                    _leftDoor.DOLocalMoveY(_initialLeft, 0);
                    _rightDoor.DOLocalMoveY(_initialRight, 0);
                }
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void ActivateDoorpc()
        {
            if (!_verticalDirection)
            {
                if (IsActivated)
                {
                    _leftDoor.DOLocalMoveX(-3.275f, _moveDuration).SetEase(Ease.Linear);
                    _rightDoor.DOLocalMoveX(1.525f, _moveDuration).SetEase(Ease.Linear);
                }
                else
                {
                    _leftDoor.DOLocalMoveX(_initialLeft, _moveDuration).SetEase(Ease.Linear);
                    _rightDoor.DOLocalMoveX(_initialRight, _moveDuration).SetEase(Ease.Linear);
                }
            }
            else
            {
                if (IsActivated)
                {
                    _leftDoor.DOLocalMoveY(2.815f, _moveDuration).SetEase(Ease.Linear);
                    _rightDoor.DOLocalMoveY(-1.31f, _moveDuration).SetEase(Ease.Linear);
                }
                else
                {
                    _leftDoor.DOLocalMoveY(_initialLeft, _moveDuration).SetEase(Ease.Linear);
                    _rightDoor.DOLocalMoveY(_initialRight, _moveDuration).SetEase(Ease.Linear);
                }
            }
        }
    }
}
