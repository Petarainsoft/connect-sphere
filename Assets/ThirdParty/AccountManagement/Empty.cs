using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

namespace AccountManagement
{
    public class Empty : AppBaseState
    {
        [SerializeField] private GameObject _accountUIBackground;

        public override void OnEnter()
        {
            base.OnEnter();
            _accountUIBackground.SetActive(false);
            ApiManager.Instance.Onlogout += ToCreateAcount;
        }

        public override void OnExit()
        {
            base.OnExit();
            _accountUIBackground.SetActive(true);
            ApiManager.Instance.Onlogout -= ToCreateAcount;
        }

        private void ToCreateAcount() => Machine.ChangeState<CreateAccountState>();
    }
}