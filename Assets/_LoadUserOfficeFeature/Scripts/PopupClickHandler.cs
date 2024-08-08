using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ConnectSphere
{
    public class PopupClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private List<GameObject> _popupObjects;

        private bool isClicked = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            isClicked = !isClicked;


            foreach (GameObject popupObject in _popupObjects)
                {
                    popupObject.SetActive(isClicked);
                  
                }
            
        }

        
    }
}
