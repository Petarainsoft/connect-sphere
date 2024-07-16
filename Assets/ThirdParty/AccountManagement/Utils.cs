using System.Text.RegularExpressions;
using Doozy.Engine.UI;

namespace AccountManagement
{
    public static class Utils
    {
        public static bool ValidateUser(string username)
        {
            return !string.IsNullOrEmpty(username)
                   && username.Length >= AppConfig.UserNameMinLength
                   && Regex.IsMatch(username, AppConfig.UserNameRegex);
        }

        public static bool ValidatePassword(string pass)
        {
            return !string.IsNullOrEmpty(pass) && pass.Length >= AppConfig.PasswordMinLength;
        }

        public static bool ValidateEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, AppConfig.EmailRegex, RegexOptions.IgnoreCase);
        }


        private static UIPopup _loadingPopup;

        public static void ShowLoading()
        {
            if ( _loadingPopup == null )
            {
                _loadingPopup = UIPopupManager.GetPopup("LoadingPopup");
            }

            _loadingPopup.Show();
        }

        public static void HideLoading()
        {
            if ( _loadingPopup != null )
            {
                _loadingPopup.Hide();
            }
        }

        public static bool ValidateResetCode(string resetcode)
        {
            return !string.IsNullOrEmpty(resetcode) && resetcode.Length == AppConfig.ResetCodeLength;
        }
    }
}