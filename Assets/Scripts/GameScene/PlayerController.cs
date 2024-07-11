using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float _speed = 5f;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private Vector2 _movementDirection;
        private float _movementSpeed;
        private bool _isMobile;
        private bool _externalInputBlocked = false;

        [Networked, OnChangedRender(nameof(OnHorizontalChanged))] public float horizontalParam { get; set; }
        [Networked, OnChangedRender(nameof(OnVerticalChanged))] public float verticalPararm { get; set; }
        [Networked, OnChangedRender(nameof(OnSpeedChanged))] public float speedParam { get; set; }

        private int _hashHorizontal = Animator.StringToHash("Horizontal");
        private int _hashVertical = Animator.StringToHash("Vertical");
        private int _hashSpeed = Animator.StringToHash("Speed");

        private void Start()
        {
            _isMobile = CheckRunOnMobile();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;

            if (GetInput<PlayerInput>(out var input))
            {
                MovementHandler(input);
                Animate();
            }
        }

        public void SetupComponents()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
        }

        private bool CheckRunOnMobile()
        {
            bool isMobile = false;
            if (Application.isMobilePlatform)
                isMobile = true;

#if UNITY_ANDROID || UNITY_IPHONE
            isMobile = true;
#endif
            return isMobile;
        }
        Vector2 refVelocity;
        private void MovementHandler(PlayerInput input)
        {
            if (!_isMobile)
            {
                SetMovement(input);
            }
            _movementSpeed = Mathf.Clamp(_movementDirection.sqrMagnitude, 0f, 1f);


            _rigidbody.velocity = _movementDirection * _movementSpeed * _speed * Runner.DeltaTime;
        }

        private void SetMovement(PlayerInput input)
        {
            _movementDirection = new Vector2(input.HorizontalInput, input.VerticalInput).normalized;
        }

        public void SetMovement(Vector2 movement)
        {
            _movementDirection = new Vector2(movement.x, movement.y);
        }

        private void Animate()
        {
            if (_animator == null || _animator.runtimeAnimatorController == null)
                return;

            speedParam = _movementSpeed;
            horizontalParam = _movementDirection.x;
            verticalPararm = _movementDirection.y;
        }

        private void OnSpeedChanged()
        {
            _animator.SetFloat(_hashSpeed, speedParam);
        }

        private void OnHorizontalChanged()
        {
            _animator.SetFloat(_hashHorizontal, horizontalParam);
        }

        private void OnVerticalChanged()
        {
            _animator.SetFloat(_hashVertical, verticalPararm);
        }
    }
}
