using RobustFSM.Base;

namespace Chat.States
{
    public class BaseCallState : MonoState
    {
        protected VivoxVideoCall ThisVideoCall => ((VivoxVideoCallFsm)SuperMachine)?.Owner;
    }
}
