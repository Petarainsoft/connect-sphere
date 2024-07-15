using System.Collections.Generic;
using System.Threading.Tasks;
using Best.HTTP;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AccountManagement
{
    public class AuthApiHandler : ServerHandler
    {
        [SerializeField] private string _registerEndpoint = "auth/register";
        [SerializeField] private string _loginEndpoint = "auth/login";
        [SerializeField] private string _testEndpoint = "auth/verify_token";
        [SerializeField] private string _requestResetPassCodeEndpoint = "auth/request-reset-password-code";
        [SerializeField] private string _resetPasswordEndpoint = "auth/reset-password-using-code";

        
        public async UniTask<bool> CheckAccessToken()
        {
            if ( string.IsNullOrEmpty(_accessToken) )
            {
                return false;
            }

            var apiUrl = $"{_serverUrl}{_testEndpoint}";
            Debug.Log(apiUrl);

            var request = HTTPRequest.CreateGet(apiUrl);
            if ( request == null ) return false;

            request.AddHeader("Authorization", _bearerAccessToken);

            try
            {
                var response = await request.GetHTTPResponseAsync();
                var apiResponse = response.ToApiResponse<VerifyTokenResult>();
                Debug.Log(apiResponse);
                return apiResponse != null && apiResponse.IsSuccess;
            }
            catch (AsyncHTTPException e)
            {
                Debug.LogError($"Request finished with error! Error: {e.Message}");
            }

            return false;
        }
        
        public async UniTask<LoginResult> Login(string username, string password)
        {
            var apiResponse = await CreatePostRequestInternal<LoginResult>($"{_serverUrl}{_loginEndpoint}",
                new Dictionary<string, object> { { "username", username }, { "password", password } });

            if ( !apiResponse.IsSuccess ) Debug.LogError(apiResponse.ResponseMessage);
            if ( apiResponse.ReturnData.error != null )
            {
                Debug.LogError(apiResponse.ReturnData.error.code);
                return apiResponse.ReturnData;
            }

            _accessToken = apiResponse.ReturnData.data.access_token;
            _bearerAccessToken = $"Bearer {_accessToken}";
            var userId = apiResponse.ReturnData.data.id;
            var _userName = apiResponse.ReturnData.data.username;

            PersisCredentials(_userName, _accessToken, userId);

            return apiResponse.ReturnData;
        }
        
        public async UniTask<RegisterResult> Register(string username, string password)
        {
            var apiResponse = await CreatePostRequestInternal<RegisterResult>($"{_serverUrl}{_registerEndpoint}",
                new Dictionary<string, object> { { "username", username }, { "password", password } });

            if ( !apiResponse.IsSuccess ) Debug.LogError(apiResponse.ResponseMessage);
            if ( apiResponse.ReturnData.error != null ) Debug.LogError(apiResponse.ReturnData.error.code);

            return apiResponse.ReturnData;
        }

        private static void PersisCredentials(string usn, string accessToken, int userId)
        {
            PlayerPrefs.SetString("token", accessToken);
            PlayerPrefs.SetString("username", usn);
            PlayerPrefs.SetInt("userId", userId);
        }

        public void ClearToken()
        {
            _accessToken = string.Empty;
            _bearerAccessToken = string.Empty;
        }

        public async Task<RequestCodeResult> RequestCode(string email)
        {
            var apiResponse = await CreatePostRequestInternal<RequestCodeResult>(
                $"{_serverUrl}{_requestResetPassCodeEndpoint}",
                new Dictionary<string, object> { { "email", email } });

            if ( !apiResponse.IsSuccess ) Debug.LogError(apiResponse.ResponseMessage);
            return apiResponse.ReturnData;
        }

        public async Task<UpdatePasswordResult> UpdatePassword(string resetCode, string password, string email)
        {
            var apiResponse = await CreatePostRequestInternal<UpdatePasswordResult>(
                $"{_serverUrl}{_resetPasswordEndpoint}",
                new Dictionary<string, object>
                {
                    { "email", email },
                    { "reset_code", resetCode },
                    { "new_password", password }
                });

            if ( !apiResponse.IsSuccess ) Debug.LogError(apiResponse.ResponseMessage);
            return apiResponse.ReturnData;
        }
    }
}