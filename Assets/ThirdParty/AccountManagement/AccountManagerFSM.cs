using System.Collections;
using System.Collections.Generic;
using AccountManagement;
using RobustFSM.Base;
using UnityEngine;

namespace ConnectSphere
{
    public class AccountManagerFSM : MonoFSM<AccountManager>
    {
        public override void AddStates()
        {
            AddState<LoginState>();
            AddState<ResetPasswordState>();
            SetInitialState<LoginState>();
        }
    }
}
