using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

namespace AccountManagement
{
    public class LoginState : AppBaseState
    {
        [Header("GameEvents")] [SerializeField]
        private string _backToIntroEventName = "BackIntroduction";

        [Header("UI")] [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _passwordInput;

        [SerializeField] private UIButton _nextButton;
        [SerializeField] private UIButton _resetPassButton;
        [SerializeField] private UIButton _backButton;

        [SerializeField] private GameObject _networkCanvas;
        

        public override void OnEnter()
        {
            base.OnEnter();
            ClearInputs();

            _nextButton.OnClick.OnTrigger.Event.AddListener(Login);
            _resetPassButton.OnClick.OnTrigger.Event.AddListener(ToResetPass);
            _backButton.OnClick.OnTrigger.Event.AddListener(ToCreateAccount);
        }

        private void ToResetPass() => Machine?.ChangeState<ResetPasswordState>();
        private void ToCreateAccount() => Machine?.ChangeState<CreateAccountState>();

        public override void OnExit()
        {
            base.OnExit();
            _nextButton.OnClick.OnTrigger.Event.RemoveListener(Login);
            _resetPassButton.OnClick.OnTrigger.Event.RemoveListener(ToResetPass);
            _backButton.OnClick.OnTrigger.Event.RemoveListener(ToCreateAccount);
        }


        private void Login()
        {
            _ = InternalLogin();
        }

        private async UniTaskVoid InternalLogin()
        {
            Utils.ShowLoading();
            
            var email = _emailInput.text.Trim();
            var password = _passwordInput.text.Trim();

            if ( !Utils.ValidateEmail(email) || !Utils.ValidatePassword(password) )
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Authentication", "Invalid email or/and password (at least 6 characters)");
                warningPopup.Show();
                
            
                Utils.HideLoading();
                return;
            }

            var loginResult = await ApiManager.Instance.AuthApi.Login(email, password);

            if ( loginResult.data != null )
            {
                _networkCanvas.SetActive(true);
                Machine.ChangeState<Empty>();
            }
            else
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Authentication", loginResult.message);
                warningPopup.Show();
            }
            
            Utils.HideLoading();
        }

        private void ClearInputs()
        {
            _emailInput.text = "";
            _passwordInput.text = "";
        }

        // protected override void OnGameMessage(GameEventMessage gameEvent)
        // {
        //     if ( gameEvent.EventName == _backToIntroEventName ) Machine.ChangeState<CleanState>();
        // }
    }
}