using RobustFSM.Base;

namespace Chat.States
{
    public class BaseCallState : MonoState
    {
        protected VideoCall ThisVideoCall => ((VideoCallFsm)SuperMachine)?.Owner;
    }
}
