using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class Emoticon : MonoBehaviour
    {
        public void OnEmoticonClicked()
        {
            Player.OnEmoticonClicked?.Invoke(transform.GetSiblingIndex());
        }
    }
}
