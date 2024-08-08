using System;
using UnityEngine;

namespace ConnectSphere
{
    [CreateAssetMenu(fileName = "BooleanEventHandler", menuName = "Events/Boolean Event Handler")]
    public class BooleanEventHandlerSO : ScriptableObject
    {
        public Action<bool> OnEventRaised;

        public void RaiseEvent(bool value)
        {
            if (OnEventRaised == null)
            {
                Debug.LogWarning($"Nothing subscribes to this {name} event.");
                return;
            }
            OnEventRaised.Invoke(value);
        }
    }
}
