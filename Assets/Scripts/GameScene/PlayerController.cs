using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace ConnectSphere
{
    public class PlayerController : NetworkBehaviour
    {
        private enum State
        {
            Normal = 0,
            BlockControl = 1,
            Busy = 2
        }

        private enum Interaction
        {
            SitDown = 0,
            SitUp = 1,
            SitLeft = 2,
            SitRight = 3,
            Door = 4,
            StickerBoard = 5,
        }

        [Header("Data")]
        [SerializeField] private float _speed = 5f;
        [Networked] public int InteractionCode { get; set; } = -1;

        [Header("Events")]
        [SerializeField] private VoidEventHandlerSO _onOpenUserInfoButtonPressed;
        [SerializeField] private BooleanEventHandlerSO _onUiInteracting;
        [SerializeField] private BooleanEventHandlerSO _onActivityPanelToggled;

        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private Interactable _interactionTarget;
        private Player _player;
        private SpriteRenderer _characterSprite;
        private Vector2 _movementDirection;
        private Vector2 refVelocity;
        private Vector2 _interactPosition;
        private float _movementSpeed;
        private bool _isMobile;
        private bool _canInteract;
        private bool _isBlockingControl;
        private bool _isReadyForActivity;
        private bool _isInActivity;

        [Networked, OnChangedRender(nameof(OnHorizontalChanged))] public float horizontalParam { get; set; }
        [Networked, OnChangedRender(nameof(OnVerticalChanged))] public float verticalPararm { get; set; }
        [Networked, OnChangedRender(nameof(OnSpeedChanged))] public float speedParam { get; set; }
        [Networked, OnChangedRender(nameof(OnSittingChanged))] public bool sittingParam { get; set; }
        private NetworkButtons previousButton { get; set; }

        private readonly int _hashHorizontal = Animator.StringToHash("Horizontal");
        private readonly int _hashVertical = Animator.StringToHash("Vertical");
        private readonly int _hashSpeed = Animator.StringToHash("Speed");
        private readonly int _hashSitting = Animator.StringToHash("Sitting");

        private void OnEnable()
        {
            _onUiInteracting.OnEventRaised += BlockControl;
            _onActivityPanelToggled.OnEventRaised += SetInActivity;
        }

        private void OnDisable()
        {
            _onUiInteracting.OnEventRaised -= BlockControl;
            _onActivityPanelToggled.OnEventRaised -= SetInActivity;
        }

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
                ProcessInput(input);
            }
        }

        public void SetupComponents()
        {
            _player = GetComponent<Player>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _characterSprite = GetComponentInChildren<SpriteRenderer>();
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
            if (_isBlockingControl)
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

        public void SetInteractionData(int code, Vector2 interactPosition = default, Interactable interactionTarget = null)  
        {
            _canInteract = code != -1 ? true : false;
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

        private void ProcessInput(PlayerInput input)
        {
            if (input.Buttons.WasPressed(previousButton, PlayerButtons.Interact) && _canInteract)
            {
                if (InteractionCode >= (int)Interaction.SitDown && InteractionCode <= (int)Interaction.SitRight)
                {
                    HandleSitting();
                }
                else if (InteractionCode == (int)Interaction.Door)
                {
                    if (_interactionTarget != null)
                    {
                        var targetDoor = _interactionTarget.GetComponent<SlidingDoorInteractable>();
                        targetDoor.ToggleDoorRpc(!targetDoor.IsActivated);
                    }
                }
                else if (InteractionCode == (int)Interaction.StickerBoard)
                {
                    if (_isBlockingControl)
                        return;

                    if (_interactionTarget != null)
                    {
                        BlockControl(true);
                        var target = _interactionTarget.GetComponent<StickerBoardInteractable>();
                        target.ActivateLocalCanvas();
                        _interactionTarget.ToggleHighlight(false);
                    }
                }
            }

            if (input.Buttons.WasPressed(previousButton, PlayerButtons.OpenUserInfo))
            {
                if (_isBlockingControl)
                    return;

                _onOpenUserInfoButtonPressed.RaiseEvent();
            }

            if (input.Buttons.WasPressed(previousButton, PlayerButtons.InviteToGame))
            {
                if (!_isReadyForActivity || _isInActivity)
                    return;

                if (_interactionTarget != null)
                {
                    _interactionTarget.SendInvite();
                }
            }

             previousButton = input.Buttons;
        }

        private void HandleSitting()
        {
            if (_isBlockingControl)
            {
                _interactionTarget.SetLimit(false, Object.InputAuthority.PlayerId);
                verticalPararm = 0;
                horizontalParam = 0;
                sittingParam = false;
                BlockControl(false);
                _isReadyForActivity = false;
            }
            else
            {
                if (_interactionTarget.LimitToOne)
                    return;

                _interactionTarget.SetLimit(true, Object.InputAuthority.PlayerId);
                _interactionTarget.ToggleHighlight(false);
                BlockControl(true);
                transform.position = _interactPosition;
                _isReadyForActivity = _interactionTarget.GetActivityAvail();

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

        private void BlockControl(bool isBlocking)
        {
            _isBlockingControl = isBlocking;
            if (isBlocking)
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }

        private void SetInActivity(bool value)
        {
            _isInActivity = value;
        }

        public string GetPlayerName()
        {
            return $"{_player.NickName}";
        }

        public Sprite GetCharacterSprite()
        {
            return _characterSprite.sprite;
        }
    }
}
