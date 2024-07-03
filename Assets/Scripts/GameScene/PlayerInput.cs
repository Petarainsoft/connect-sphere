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
        private Vector2 _movementDirection;
        private float _movementSpeed;
        private bool _isMobile;
        private bool _externalInputBlocked = false;

        private int _hashHorizontal = Animator.StringToHash("Horizontal");
        private int _hashVertical = Animator.StringToHash("Vertical");
        private int _hashSpeed = Animator.StringToHash("Speed");

        private void Start()
        {
            _isMobile = CheckRunOnMobile();
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

        public void SetupComponents(GameObject targetObject)
        {
            _rigidbody = targetObject.GetComponent<Rigidbody2D>();
            _animator = targetObject.GetComponentInChildren<Animator>();
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

        private void MovementHandler()
        {
            if (_externalInputBlocked)
            {
                _movementSpeed = 0;
                return;
            }

            if (!_isMobile)
            {
                SetMovement();
            }
            _movementSpeed = Mathf.Clamp(_movementDirection.sqrMagnitude, 0f, 1f);
        }

        private void SetMovement()
        {
            _movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }

        public void SetMovement(Vector2 movement)
        {
            _movementDirection = new Vector2(movement.x, movement.y);
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
