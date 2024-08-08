using Cysharp.Threading.Tasks;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class RocPapSci : ActivityController
    {
        public enum Phase
        {
            None = 0,
            Begin = 1,
            Ready = 2,
            Turn = 3,
            Show = 4,
            End = 5
        }

        private enum Shape
        {
            Rock = 0,
            Paper = 1,
            Scissors = 2
        }

        [Header("Specific")]
        [SerializeField] private TMP_Text _leftUserName;
        [SerializeField] private TMP_Text _rightUserName;
        [SerializeField] private TMP_Text _textRound;
        [SerializeField] private TMP_Text _textResult;
        [SerializeField] private TMP_Text _textCountdown;
        [SerializeField] private TMP_Text _textPhase;
        [SerializeField] private Image _leftUserAvatar;
        [SerializeField] private Image _rightUserAvatar;
        [SerializeField] private GameObject _leftHandBubble;
        [SerializeField] private GameObject _rightHandBubble;
        [SerializeField] private Image _leftFinalShape;
        [SerializeField] private Image _rightFinalShape;
        [SerializeField] private Button[] _buttons = new Button[3];

        [Header("Sprites")]
        [SerializeField] private Sprite[] _shapeSprites = new Sprite[3];

        [Networked, OnChangedRender(nameof(OnPhaseChanged))] public Phase CurrentPhase { get; set; }
        [Networked, OnChangedRender(nameof(OnRoundChanged))] public int CurrentRound { get; set; } = 0;
        [Networked] private TickTimer _timer { get; set; }
        [Networked, OnChangedRender(nameof(OnTimerChanged))] private float _remainingTime { get; set; }
        [Networked] public int LeftShape { get; set; }
        [Networked] public int RightShape { get; set; }

        private PlayerRef _leftUser;
        private PlayerRef _rightUser;
        private int _selectedShape = 0;
        private int _leftUserScore = 0;
        private int _rightUserScore = 0;

        private const int TOTAL_ROUNDS = 3;
        private const float READY_DURATION = 2;
        private const float TURN_DURATION = 8;
        private const float SHOW_DURATION = 3;

        private void OnEnable()
        {
            CurrentPhase = Phase.Begin;
            CurrentRound = 0;
            _textRound.text = "0 - 0";
        }

        private void OnDisable()
        {
            ResetAll();
        }

        public override async void FixedUpdateNetwork()
        {
            switch (CurrentPhase)
            {
                case Phase.Begin:
                    ResetAll();
                    await PrepareBegin();
                    break;
                case Phase.Ready:
                    UpdateReadyDisplay();
                    break;
                case Phase.Turn:
                    UpdateTurnDisplay();
                    break;
                case Phase.Show:
                    UpdateShowDisplay();
                    break;
            }
        }

        private void ResetAll()
        {
            _textResult.text = "0 - 0";
            _textRound.text = "Round";
            _textPhase.text = "";
            _leftUserScore = 0;
            _rightUserScore = 0;
            _leftHandBubble.SetActive(false);
            _rightHandBubble.SetActive(false);
        }

        private async UniTask PrepareBegin()
        {
            if (!Object.HasStateAuthority)
                return;
            await UniTask.WaitForSeconds(0.1f);
            LeftShape = 0;
            RightShape = 0;
            CurrentRound = 1;
            CurrentPhase = Phase.Ready;
            _timer = TickTimer.CreateFromSeconds(Runner, READY_DURATION);
        }

        private void UpdateReadyDisplay()
        {
            if (!Object.HasStateAuthority)
                return;

            _remainingTime = (float)_timer.RemainingTime(Runner);

            if (_timer.ExpiredOrNotRunning(Runner) == false)
                return;
            
            CurrentPhase = Phase.Turn;
            _timer = TickTimer.CreateFromSeconds(Runner, TURN_DURATION);
        }

        private void UpdateTurnDisplay()
        {
            if (!Object.HasStateAuthority)
                return;

            _remainingTime = (float)_timer.RemainingTime(Runner);

            if (_timer.ExpiredOrNotRunning(Runner) == false)
                return;

            CurrentPhase = Phase.Show;
            _timer = TickTimer.CreateFromSeconds(Runner, SHOW_DURATION);
        }

        private void UpdateShowDisplay()
        {
            if (!Object.HasStateAuthority)
                return;

            _remainingTime = (float)_timer.RemainingTime(Runner);

            if (_timer.ExpiredOrNotRunning(Runner) == false)
                return;

            if (CurrentRound < TOTAL_ROUNDS)
            {
                CurrentRound++;
                CurrentPhase = Phase.Turn;
                _timer = TickTimer.CreateFromSeconds(Runner, TURN_DURATION);
            }
            else
            {
                CurrentPhase = Phase.End;
                GameHasEnded();
            }
        }

        private void GameHasEnded()
        {
            _textPhase.text = "WINS!";
            _textCountdown.text = string.Empty;
        }

        private void OnPhaseChanged()
        {
            switch (CurrentPhase)
            {
                case Phase.Ready:
                    FillData();
                    break;
                case Phase.Turn:
                    HandleTurn();
                    break;
                case Phase.Show:
                    CalculateWinner((Shape)LeftShape, (Shape)RightShape);
                    ShowResult();
                    break;
                case Phase.End:
                    HandleEnding();
                    break;
            }
        }

        private void FillData()
        {
            _textPhase.text = "Get Ready";

            foreach (int playerId in PlayingUserIds)
            {
                var playerController = GetPlayerControllerById(playerId);
                if (playerController != null)
                {
                    if (playerController.InteractionCode == 2)  // right user
                    {
                        _rightUser = GetPlayerRefById(playerId);
                        _rightUserName.text = playerController.GetPlayerName();
                        _rightUserAvatar.sprite = playerController.GetCharacterSprite();
                    }
                    if (playerController.InteractionCode == 3)  // left user
                    {
                        _leftUser = GetPlayerRefById(playerId);
                        _leftUserName.text = playerController.GetPlayerName();
                        _leftUserAvatar.sprite = playerController.GetCharacterSprite();
                    }
                }
            }
        }

        private void HandleTurn()
        {
            _textPhase.text = "PICK ONE!";

            _leftHandBubble.SetActive(false);
            _rightHandBubble.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            foreach (Button button in _buttons)
            {
                button.interactable = true;
            }
        }

        private void ShowResult()
        {
            _textPhase.text = "SHOW!";

            foreach (Button button in _buttons)
            {
                button.interactable = false;
            }
            _leftFinalShape.sprite = _shapeSprites[LeftShape];
            _rightFinalShape.sprite = _shapeSprites[RightShape];
            _leftHandBubble.SetActive(true);
            _rightHandBubble.SetActive(true);
            _textResult.text = $"{_leftUserScore} - {_rightUserScore}";
        }

        private void CalculateWinner(Shape leftShape, Shape rightShape)
        {
            if (leftShape == Shape.Rock && rightShape == Shape.Scissors)
            {
                _leftUserScore++;
            }
            else if (leftShape == Shape.Rock && rightShape == Shape.Paper)
            {
                _rightUserScore++;
            }
            else if (leftShape == Shape.Paper && rightShape == Shape.Rock)
            {
                _leftUserScore++;
            }
            else if (leftShape == Shape.Paper && rightShape == Shape.Scissors)
            {
                _rightUserScore++;
            }
            else if (leftShape == Shape.Scissors && rightShape == Shape.Paper)
            {
                _leftUserScore++;
            }
            else if (leftShape == Shape.Scissors && rightShape == Shape.Rock)
            {
                _rightUserScore++;
            }
        }

        public void HandleSelectedShape(int shapeIndex)
        {
            if (Runner.LocalPlayer == _leftUser)
            {
                SetShapeRpc(true, shapeIndex);
            }
            else if (Runner.LocalPlayer == _rightUser)
            {
                SetShapeRpc(false, shapeIndex);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void SetShapeRpc(bool isLeftShape, int shapeIndex)
        {
            if (isLeftShape)
            {
                LeftShape = shapeIndex;
            }
            else
            {
                RightShape = shapeIndex;
            }
        }

        private void HandleEnding()
        {
            foreach (Button button in _buttons)
            {
                button.interactable = false;
            }
            _textCountdown.text = string.Empty;
            if (_leftUserScore > _rightUserScore)
            {
                _textPhase.text = $"{_leftUserName.text} WON!";
            }
            else if (_leftUserScore < _rightUserScore)
            {
                _textPhase.text = $"{_rightUserName.text} WON!";
            }
            else
            {
                _textPhase.text = "DRAW";
            }
        }

        private void OnRoundChanged()
        {
            _textRound.text = "Round " + CurrentRound;
        }

        private void OnTimerChanged()
        {
            _textCountdown.text = Mathf.RoundToInt(_remainingTime).ToString();
        }

        private PlayerController GetPlayerControllerById(int playerId)
        {
            var networkObject = Runner.GetPlayerObject(GetPlayerRefById(playerId));
            return networkObject.GetComponent<PlayerController>();
        }

        private PlayerRef GetPlayerRefById(int playerId)
        {
            foreach (PlayerRef playerRef in Runner.ActivePlayers)
            {
                if (Runner.GetPlayerObject(playerRef) is NetworkObject playerObject && playerObject.InputAuthority.PlayerId == playerId)
                {
                    return playerRef;
                }
            }
            return PlayerRef.None;
        }
    }
}
