using System;
using UnityEngine;
using UnityEngine.Android;

namespace Chat
{
    public class PermissionHelper
    {
        private readonly string requestPermission;
        private PermissionCallbacks permissionCallbacks;
        public Action<bool> OnPermissionResult;
        
        public PermissionHelper(string requestPermission)
        {
            this.requestPermission = requestPermission;
        }
        public void Ask()
        {
            if ( Permission.HasUserAuthorizedPermission(requestPermission) )
            {
                Debug.Log($"{requestPermission} is granted");
                OnPermissionResult?.Invoke(true);
            }
            else
            {
                SubscribeCameraPermissionEvent();
                Permission.RequestUserPermission(requestPermission, permissionCallbacks);
            }
        }

        internal void DeniedAndDontAskAgain(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
            permissionCallbacks.PermissionDenied -= Denied;
            permissionCallbacks.PermissionGranted -= Granted;
            permissionCallbacks.PermissionDeniedAndDontAskAgain -= DeniedAndDontAskAgain;
            OnPermissionResult?.Invoke(false);
        }

        internal void Granted(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
            permissionCallbacks.PermissionDenied -= Denied;
            permissionCallbacks.PermissionGranted -= Granted;
            permissionCallbacks.PermissionDeniedAndDontAskAgain -= DeniedAndDontAskAgain;
            
            OnPermissionResult?.Invoke(true);
        }

        internal void Denied(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
            permissionCallbacks.PermissionDenied -= Denied;
            permissionCallbacks.PermissionGranted -= Granted;
            permissionCallbacks.PermissionDeniedAndDontAskAgain -= DeniedAndDontAskAgain;
            OnPermissionResult?.Invoke(false);
        }
        
        private void SubscribeCameraPermissionEvent()
        {
            permissionCallbacks = new PermissionCallbacks();
            permissionCallbacks.PermissionDenied += Denied;
            permissionCallbacks.PermissionGranted += Granted;
            permissionCallbacks.PermissionDeniedAndDontAskAgain += DeniedAndDontAskAgain;
        }
    }
}