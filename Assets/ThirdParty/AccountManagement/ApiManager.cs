using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectSphere;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AccountManagement
{
    public class ApiManager : ConnectSphere.Singleton<ApiManager>
    {
        [Header("Base URL for all handlers")]
        [SerializeField] private string _baseUrl;
        
        [Space(20)]
        [SerializeField]
        private AuthApiHandler _authApiHandler;
        public AuthApiHandler AuthApi => _authApiHandler;
        
        [SerializeField]
        private NoteApiHandler _noteApiHandler;
        public NoteApiHandler NoteApi => _noteApiHandler;
        
        [SerializeField]
        private ProfileApiHandler _profileApiHandler;
        public ProfileApiHandler ProfileApi => _profileApiHandler;

        public int UserId => PlayerPrefs.GetInt("userId");

        protected override void Awake()
        {
            base.Awake();
            if ( string.IsNullOrEmpty(_baseUrl) ) return;
            var handlers = GetComponents<ServerHandler>();
            if ( handlers == null || handlers.Length == 0 ) return;
            foreach (var handler in handlers) if ( handler != null ) handler.SetBaseUrl(_baseUrl);
        }

        public Action Onlogout;
        
        public async void Logout()
        {
            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.DeleteKey("userId");

            if ( AuthApi != null ) AuthApi.ClearToken();
            Debug.Log("Logged out");
            Onlogout?.Invoke();
        }

        public async UniTask<bool> CheckAuthen()
        {
            return await AuthApi.CheckAccessToken();
        }
    }
}
