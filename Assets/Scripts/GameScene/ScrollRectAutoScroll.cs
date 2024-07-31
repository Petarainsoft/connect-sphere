using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ConnectSphere
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectAutoScroll : MonoBehaviour
    {
        public float scrollSpeed = 20f;
        [Space]
        [Header("Fonts")]
        [SerializeField] private TMP_FontAsset _boldFont;

        private List<Selectable> m_Selectables = new List<Selectable>();
        private ScrollRect m_ScrollRect;

        void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
        }

        void Start()
        {
            if (m_ScrollRect)
            {
                m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
            }
            ReformatElements();
            ScrollToSelected();
        }

        void ScrollToSelected()
        {
            int selectedIndex = -1;
            Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

            if (selectedElement)
            {
                selectedIndex = m_Selectables.IndexOf(selectedElement);
            }
            if (selectedIndex > -1)
            {
                m_ScrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
            }
        }

        void ReformatElements()
        {
            foreach (Selectable selectable in m_Selectables)
            {
                if (selectable.name.Contains("<b>"))
                {
                    selectable.GetComponentInChildren<TMP_Text>().font = _boldFont;
                }
            }
        }
    }
}
