using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class HorizontalLayout : MonoBehaviour
    {
        public RectTransform contentTransform;
        public float rowHeight = 100f; // Set the desired height

        void Start()
        {
            HorizontalLayoutGroup layoutGroup = contentTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.spacing = 10; // Adjust the spacing between images if needed

            // Adjust the size of the content transform to fit the images
            contentTransform.sizeDelta = new Vector2(contentTransform.childCount * (rowHeight + layoutGroup.spacing), rowHeight);

            foreach (RectTransform child in contentTransform)
            {
                child.sizeDelta = new Vector2(rowHeight, rowHeight); // Set the size of each RawImage
            }
        }
    }
}
