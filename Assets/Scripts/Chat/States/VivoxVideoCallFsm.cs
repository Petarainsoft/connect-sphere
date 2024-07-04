using RobustFSM.Base;

namespace Chat.States
{
    public class VivoxVideoCallFsm : MonoFSM<VivoxVideoCall>
    {
        public override void AddStates()
        {
            AddState<InCall>();
            AddState<StartCall>();
            
            SetInitialState<StartCall>();
        }
    }
}