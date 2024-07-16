using System;
using System.Collections;
using Cysharp.Threading.Tasks;
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
        
        private bool CheckPermissionAndRaiseCallbackIfGranted(UserAuthorization authenticationType)
        {
            if (Application.HasUserAuthorization(authenticationType))
            {
                OnPermissionResult?.Invoke(true);

                return true;
            }
            return false;
        }

        private async UniTaskVoid AskForPermissionIfRequired(UserAuthorization authenticationType, Action authenticationGrantedAction)
        {
            if (!CheckPermissionAndRaiseCallbackIfGranted(authenticationType))
            {
                await Application.RequestUserAuthorization(authenticationType);
                if ( !CheckPermissionAndRaiseCallbackIfGranted(authenticationType) )
                {
                    OnPermissionResult?.Invoke(false);
                }
                else
                {
                    OnPermissionResult?.Invoke(true);
                }

                var dd = Enum.Parse(typeof(UserAuthorization), ToString());
            }
        }
        
        public void Ask()
        {
            #if UNITY_IOS
            AskForPermissionIfRequired( Enum.Parse(typeof(UserAuthorization), requestPermission ));
            #else
            
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
            #endif
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