using AccountManagement;
using RobustFSM.Base;

namespace ConnectSphere
{
    public class AccountManagerFSM : MonoFSM<AccountManager>
    {
        public override void AddStates()
        {
            AddState<CheckingAuth>();
            AddState<CreateAccountState>();
            AddState<LoginState>();
            AddState<ResetPasswordState>();
            AddState<Empty>();
            SetInitialState<CheckingAuth>();
        }
    }
}
