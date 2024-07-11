using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ConnectSphere
{
    public class AvatarImage : MonoBehaviour, IPointerClickHandler
    {
        private int _index = 0;
        private void Start()
        {
            _index = transform.GetSiblingIndex();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MenuManager.OnAvatarImageClicked?.Invoke(_index);
        }
    }
}
