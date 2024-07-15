using System.Collections;
using System.Collections.Generic;
using ConnectSphere;
using UnityEngine;

namespace AccountManagement
{
    public class ApiManager : Singleton<ApiManager>
    {
        [Header("Base URL for all handlers")]
        [SerializeField] private string _baseUrl;
        
        [Space(20)]
        [SerializeField]
        private AuthApiHandler _authApiHandler;
        public AuthApiHandler AuthApi => _authApiHandler;

        public int UserId => PlayerPrefs.GetInt("userId");

        protected override void Awake()
        {
            base.Awake();
            if ( string.IsNullOrEmpty(_baseUrl) ) return;
            var handlers = GetComponents<ServerHandler>();
            if ( handlers == null || handlers.Length == 0 ) return;
            foreach (var handler in handlers) if ( handler != null ) handler.SetBaseUrl(_baseUrl);
        }
        
        public void Logout()
        {
            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.DeleteKey("userId");

            if ( AuthApi != null ) AuthApi.ClearToken();
            Debug.Log("Logged out");
        }
    }
}