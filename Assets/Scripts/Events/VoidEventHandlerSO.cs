using System;
using UnityEngine;

namespace ConnectSphere
{
    [CreateAssetMenu(fileName = "VoidEventHandler", menuName = "Events/Void Event Handler")]
    public class VoidEventHandlerSO : ScriptableObject
    {
        public Action OnEventRaised;

        public void RaiseEvent()
        {
            if (OnEventRaised == null)
            {
                Debug.LogWarning($"Nothing subscribes to this {name} event.");
                return;
            }
            OnEventRaised.Invoke();
        }
    }
}
