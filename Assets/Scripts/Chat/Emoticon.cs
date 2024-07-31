using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class Emoticon : MonoBehaviour
    {
        [SerializeField] private IntegerEventHandlerSO _onEmoticonClicked;

        public void OnEmoticonClicked()
        {
            _onEmoticonClicked.RaiseEvent(transform.GetSiblingIndex());
        }
    }
}
