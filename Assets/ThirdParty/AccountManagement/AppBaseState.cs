using ConnectSphere;
using Doozy.Engine.UI;
using RobustFSM.Base;
using UnityEngine;

namespace AccountManagement
{
    public abstract class AppBaseState : MonoState
    {
        [SerializeField] protected UIView _stateUIView;

        protected AccountManager Manager => ((AccountManagerFSM)SuperMachine).Owner;

        public override void OnEnter()
        {
            base.OnEnter();
            if ( _stateUIView != null ) _stateUIView.Show();
        }

        public override void OnExit()
        {
            base.OnExit();
            if ( _stateUIView != null ) _stateUIView.Hide();
        }
    }
}