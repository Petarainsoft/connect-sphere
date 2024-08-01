using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    [CreateAssetMenu(fileName = "DoubleEventHandler", menuName = "Events/Double String Event Handler")]
    public class DoubleStringEventHandlerSO : ScriptableObject
    {
        public Action<string, string> OnEventRaised;

        public void RaiseEvent(string value1, string value2)
        {
            if (OnEventRaised == null)
            {
                Debug.LogWarning($"Nothing subscribes to this {name} event.");
                return;
            }
            OnEventRaised.Invoke(value1, value2);
        }
    }
}
