using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

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
        private Vector2 refVelocity;
        public int InteractionCode { get; set; } = -1;
        public bool CanInteract { get; set; }
        private bool _isInInteraction;
        private Vector2 _interactPosition;
        private GameObject _interactionTarget;
        private bool _blockMoving;

        [Networked, OnChangedRender(nameof(OnHorizontalChanged))] public float horizontalParam { get; set; }
        [Networked, OnChangedRender(nameof(OnVerticalChanged))] public float verticalPararm { get; set; }
        [Networked, OnChangedRender(nameof(OnSpeedChanged))] public float speedParam { get; set; }
        [Networked, OnChangedRender(nameof(OnSittingChanged))] public bool sittingParam { get; set; }
        private NetworkButtons previousButton { get; set; }

        private int _hashHorizontal = Animator.StringToHash("Horizontal");
        private int _hashVertical = Animator.StringToHash("Vertical");
        private int _hashSpeed = Animator.StringToHash("Speed");
        private int _hashSitting = Animator.StringToHash("Sitting");

        private void Start()
        {
            _isMobile = CheckRunOnMobile();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput<PlayerInput>(out var input))
            {
                MovementHandler(input);
                Animate();
                Interact(input);
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
        
        private void MovementHandler(PlayerInput input)
        {
            if (_blockMoving)
                return;

            if (!_isMobile)
            {
                SetMovement(input);
            }
            _movementSpeed = Mathf.Clamp(_movementDirection.sqrMagnitude, 0f, 1f);


            _rigidbody.velocity = _movementDirection * _movementSpeed * _speed * Runner.DeltaTime;
        }

        // keyboard input
        private void SetMovement(PlayerInput input)
        {
            _movementDirection = new Vector2(input.HorizontalInput, input.VerticalInput).normalized;
        }

        // virtual joystick input
        public void SetMovement(Vector2 movement)
        {
            _movementDirection = new Vector2(movement.x, movement.y);
        }

        public void SetInteractionData(int code, Vector2 interactPosition = default, GameObject interactionTarget = null)  
        {
            CanInteract = code != -1 ? true : false;
            InteractionCode = code;
            _interactPosition = interactPosition;
            _interactionTarget = code != -1 ? interactionTarget : null;
        }

        private void Animate()
        {
            if (_animator == null || _animator.runtimeAnimatorController == null)
                return;

            
            speedParam = _movementSpeed;
            if (_movementDirection != Vector2.zero)
            {
                horizontalParam = _movementDirection.x;
                verticalPararm = _movementDirection.y;
            }
        }

        private void Interact(PlayerInput input)
        {
            if (input.Buttons.WasPressed(previousButton, PlayerButtons.Interact) && CanInteract)
            {
                if (InteractionCode >= 0 && InteractionCode <= 3)
                {
                    HandleSitting();
                }
                else if (InteractionCode == 4)
                {
                    if (_interactionTarget != null)
                    {
                        var targetDoor = _interactionTarget.GetComponent<SlidingDoorInteractable>();
                        targetDoor.ActivateDoorpc();
                    }
                }
            }
            previousButton = input.Buttons;
        }

        private void HandleSitting()
        {
            if (_isInInteraction)
            {
                verticalPararm = 0;
                horizontalParam = 0;
                sittingParam = false;
                _isInInteraction = false;
                _blockMoving = false;
            }
            else
            {
                _rigidbody.velocity = Vector2.zero;
                _isInInteraction = true;
                _blockMoving = true;
                transform.position = _interactPosition;

                switch (InteractionCode)
                {
                    case 0:  // sit down
                        verticalPararm = -1;
                        horizontalParam = 0;
                        sittingParam = true;
                        break;
                    case 1:  // sit up
                        verticalPararm = 1;
                        horizontalParam = 0;
                        sittingParam = true;
                        break;
                    case 2:  // sit left
                        verticalPararm = 0;
                        horizontalParam = -1;
                        sittingParam = true;
                        break;
                    case 3:  // sit right
                        verticalPararm = 0;
                        horizontalParam = 1;
                        sittingParam = true;
                        break;
                }
            }
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

        private void OnSittingChanged()
        {
            _animator.SetBool(_hashSitting, sittingParam);
        }
    }
}
