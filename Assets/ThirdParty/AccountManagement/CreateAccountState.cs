using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AccountManagement
{
    public class CreateAccountState : AppBaseState
    {
        [Header("GameEvents")]
        [SerializeField] private string _loginSelectedEventName = "LoginSelected";
        [SerializeField] private string _backToIntroEventName = "BackIntroduction";
        
        [Header("UI")]
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private TMP_InputField _confirmPasswordInput;
        
        [Header("Button")]
        [SerializeField] private UIButton _createAccountButton;
        
        [Header("Agreement")]
        [SerializeField] private Toggle _agreementTosToggle;
        [SerializeField] private Toggle _agreementDataPolicyToggle;

        [SerializeField] private  UIButton _toLoginBtn;
        

        public override void OnEnter()
        {
            base.OnEnter();
            ClearInputs();
            
            _createAccountButton.OnClick.OnTrigger.Event.AddListener(CreateAccount);
            _toLoginBtn.OnClick.OnTrigger.Event.AddListener(ToLogin);
        }
        
        public override void OnExit()
        {
            base.OnExit();
            _createAccountButton.OnClick.OnTrigger.Event.RemoveListener(CreateAccount);
            _toLoginBtn.OnClick.OnTrigger.Event.RemoveListener(ToLogin);
        }


        private void CreateAccount()
        {
            _ = CreateAccountAsync();
        }

        private void ToLogin() => Machine?.ChangeState<LoginState>();

        private async UniTaskVoid CreateAccountAsync()
        {
            Utils.ShowLoading();
            
 
            var password = _passwordInput.text.Trim();
            var confirmPassword = _confirmPasswordInput.text.Trim();
            var email = _emailInput.text.Trim();

            if ( !Utils.ValidatePassword(password) || !Utils.ValidateEmail(email))
            {
                Utils.HideLoading();
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Create Account", "Please enter a valid email and/or password (6 digits)");
                warningPopup.Show();
                return;
            }
            
            if ( password != confirmPassword)
            {
                Utils.HideLoading();
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Create Account", "Passwords are not matched");
                warningPopup.Show();
                return;
            }

            var registerR = await ApiManager.Instance.AuthApi.Register(email, password);

            if ( registerR.error == null ) // successful
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Register", registerR.message);
                warningPopup.HideOnClickOverlay = false;
                warningPopup.HideOnClickAnywhere = false;
                warningPopup.HideOnClickContainer = false;
                warningPopup.Data.SetButtonsCallbacks(()=>
                {
                    Machine.ChangeState<LoginState>();
                    warningPopup.Hide();
                });
                warningPopup.Show();
            }
            else
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Register", registerR.message);
                warningPopup.Show();
            }
            
            Utils.HideLoading();
        }

        private void ClearInputs()
        {
            _emailInput.text = "";
            _passwordInput.text = "";
            _confirmPasswordInput.text = "";
            _agreementTosToggle.isOn = false;
            _agreementDataPolicyToggle.isOn = false;
        }

        // protected override void OnGameMessage(GameEventMessage gameEvent)
        // {
        //     if (gameEvent.EventName == _loginSelectedEventName) Machine.ChangeState<LoginState>();
        //     if (gameEvent.EventName == _backToIntroEventName) Machine.ChangeState<CleanState>();
        // }
    }
}