using RobustFSM.Base;

namespace Chat.States
{
    public class VideoCallFsm : MonoFSM<VideoCall>
    {
        public override void AddStates()
        {
            AddState<InCall>();
            AddState<StartCall>();
            
            SetInitialState<StartCall>();
        }
    }
}
