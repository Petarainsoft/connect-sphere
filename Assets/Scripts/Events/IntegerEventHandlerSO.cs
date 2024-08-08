using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    [CreateAssetMenu(fileName = "IntegerEventHandler", menuName = "Events/Integer Event Handler")]
    public class IntegerEventHandlerSO : ScriptableObject
    {
        public Action<int> OnEventRaised;

        public void RaiseEvent(int value)
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
