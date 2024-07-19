using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace AccountManagement
{
    public class CheckingAuth : AppBaseState
    {
       

        public override void OnEnter()
        {
            base.OnEnter();

            CheckAuthen();

        }

        private async UniTaskVoid CheckAuthen()
        {
            var isauthen = await ApiManager.Instance.AuthApi.CheckAccessToken();
            if ( isauthen )
            {
                Manager.GotoNetworkSelect();
                Machine.ChangeState<Empty>();
            }
            else
            {
                await UniTask.DelayFrame(1);
                Machine.ChangeState<CreateAccountState>();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}