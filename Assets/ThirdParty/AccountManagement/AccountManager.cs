using System;
using System.Collections;
using System.Collections.Generic;
using AccountManagement;
using UnityEngine;

namespace ConnectSphere
{
    [RequireComponent(typeof(AccountManagerFSM))]
    public class AccountManager : MonoBehaviour
    {
        [SerializeField] private AccountManagerFSM _accountManagerFsm;
        [SerializeField] private ApiManager _apiManager;
        [SerializeField] private GameObject _networkCanvas;

        private void Awake()
        {
            _networkCanvas.SetActive(false);
        }

        public void GotoNetworkSelect()
        {
            _networkCanvas.SetActive(true);
        }
    }
}
