using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

namespace AccountManagement
{
    public class ResetPasswordState : AppBaseState
    {
        [Header("GameEvents")] [SerializeField]
        private string _backToLogin = "BackToLogin";

        [Header("UI")] 
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _resetCodeInput;
        [SerializeField] private TMP_InputField _passwordInput;

        [SerializeField] private UIButton _requestCodeButton;
        [SerializeField] private UIButton _resetButton;
        [SerializeField] private UIButton _backButton;

        [SerializeField] private GameObject _loading;
        

        private void SetUI(bool sendingCode)
        {
            _emailInput.gameObject.SetActive(sendingCode);
            _resetCodeInput.gameObject.SetActive(!sendingCode);
            _passwordInput.gameObject.SetActive(!sendingCode);
            
            _requestCodeButton.gameObject.SetActive(sendingCode);
            _resetButton.gameObject.SetActive(!sendingCode);
        }
 

        public override void OnEnter()
        {
            base.OnEnter();
            ClearInputs();

            _requestCodeButton.OnClick.OnTrigger.Event.AddListener(RequestCode);
            _resetButton.OnClick.OnTrigger.Event.AddListener(ResetPassword);
            _backButton.OnClick.OnTrigger.Event.AddListener(BackToLogin);

            SetUI(sendingCode: true);
        }

        private void BackToLogin() => Machine?.ChangeState<LoginState>();

        public override void OnExit()
        {
            base.OnExit();
            _requestCodeButton.OnClick.OnTrigger.Event.RemoveListener(RequestCode);
            _resetButton.OnClick.OnTrigger.Event.RemoveListener(ResetPassword);
            _backButton.OnClick.OnTrigger.Event.RemoveListener(BackToLogin);
        }

        private void ResetPassword()
        {
            _ = ResetPasswordAsync();
        }

        private async UniTaskVoid ResetPasswordAsync()
        {
            _loading.SetActive(true);
            
            var resetCode = _resetCodeInput.text.Trim();
            var password = _passwordInput.text.Trim();
            var email = _emailInput.text.Trim();

            if ( !Utils.ValidatePassword(password) || !Utils.ValidateResetCode(resetCode))
            {
                Utils.HideLoading();
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Reset password", "Please enter a valid password and/or reset code (6 digits");
                warningPopup.Show();
                return;
            }

            var updatePassResult = await ApiManager.Instance.AuthApi.UpdatePassword(resetCode, password, email);

            if ( updatePassResult.error == null ) // successful
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Reset password", updatePassResult.message);
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
                warningPopup.Data.SetLabelsTexts("Reset password", updatePassResult.message);
                warningPopup.Show();
                
                SetUI(false);
            }
            
            Utils.HideLoading();
        }

        private void RequestCode()
        {
            _ = RequestCodeAsync();
        }

        private async UniTaskVoid RequestCodeAsync()
        {
            Utils.ShowLoading();
            
            var email = _emailInput.text.Trim();

            if ( !Utils.ValidateEmail(email))
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Invalid email", "Please enter a valid email");
                warningPopup.Show();
                Utils.HideLoading();
                return;
            }

            var requestCodeResult = await ApiManager.Instance.AuthApi.RequestCode(email);

            if ( requestCodeResult.error == null )
            {
                
                SetUI(false);
            }
            else
            {
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Request failed", requestCodeResult.message);
                warningPopup.Show();
            }
            
            Utils.HideLoading();
        }

        private void ClearInputs()
        {
            _emailInput.text = "";
            _passwordInput.text = "";
            _resetCodeInput.text = "";
        }
    }
}