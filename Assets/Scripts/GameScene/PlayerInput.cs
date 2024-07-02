using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private SpriteRenderer _renderer;
        private Vector2 _movementDirection;
        private float _movementSpeed;

        private bool _externalInputBlocked = false;

        private int _hashHorizontal = Animator.StringToHash("Horizontal");
        private int _hashVertical = Animator.StringToHash("Vertical");
        private int _hashSpeed = Animator.StringToHash("Speed");

        private void Awake()
        {
            SetupComponents();
        }

        private void Update()
        {
            MovementHandler();
            Animate();
        }

        private void FixedUpdate()
        {
            _rigidbody.velocity = _movementDirection * _movementSpeed * _speed * Time.deltaTime;
        }

        public void SetupComponents()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void MovementHandler()
        {
            if (_externalInputBlocked)
            {
                _movementSpeed = 0;
                return;
            }

            _movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            _movementSpeed = Mathf.Clamp(_movementDirection.sqrMagnitude, 0f, 1f);
            _movementDirection.Normalize();
        }

        private void Animate()
        {
            _animator.SetFloat(_hashSpeed, _movementSpeed);
            _animator.SetFloat(_hashHorizontal, _movementDirection.x);
            _animator.SetFloat(_hashVertical, _movementDirection.y);
        }

        public bool HaveControl()
        {
            return !_externalInputBlocked;
        }

        public void BlockControl()
        {
            _externalInputBlocked = true;
        }

        public void GainControl()
        {
            _externalInputBlocked = false;
        }
    }
}
