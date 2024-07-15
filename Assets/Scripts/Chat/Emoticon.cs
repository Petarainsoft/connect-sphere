using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class Emoticon : MonoBehaviour
    {
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void OnEmoticonClicked()
        {
            Debug.Log("ABC");
            Player.OnEmoticonClicked?.Invoke(_image.sprite);
        }
    }
}
