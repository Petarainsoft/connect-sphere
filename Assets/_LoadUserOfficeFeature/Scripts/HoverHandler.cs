using UnityEngine;
using UnityEngine.EventSystems;

namespace ConnectSphere
{
    public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string _hoverColor = "66665f";
        [SerializeField] private GameObject _hoverPopupObject; 
        // Start is called before the first frame update
       
        public void OnPointerEnter(PointerEventData eventData)
        {
            _hoverPopupObject.SetActive(true);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hoverPopupObject.SetActive(false);

        }
    }
}
