using UnityEngine;
using UnityEngine.Android;

namespace Chat.States
{
    public class StartCall : BaseCallState
    {
        [Tooltip("Time in seconds to wait for permissions.")]
        public float permissionRequestTimeout = 5.0f;
        
        
        private PermissionHelper _cameraHelper = new PermissionHelper(Permission.Camera);
        private PermissionHelper _microphoneHelper = new PermissionHelper(Permission.Microphone);



        public override void OnEnter()
        {
            base.OnEnter();
            
            _cameraHelper.OnPermissionResult = AfterRequestCamPermission;
            _microphoneHelper.OnPermissionResult = AfterRequestPermission;
            
            _cameraHelper.Ask();
            _microphoneHelper.Ask();
        }

        private void AfterRequestPermission(bool requestOk)
        {
            Debug.Log($"Request Permission with {requestOk}");
        }

        private void AfterRequestCamPermission(bool permissionDone)
        {
            if ( permissionDone )
            {
                ThisVideoCall.StartMyCamera();
            }
        }
    }
}