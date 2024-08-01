using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class SplitLayout : MonoBehaviour
    {
        public RectTransform leftPanel;
        public RectTransform rightPanel;
        public float leftPanelWidth = 200f; // Set the desired width of the left panel

        private RectTransform selectedImage;

        void Start()
        {
            VerticalLayoutGroup leftLayoutGroup = leftPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            leftLayoutGroup.childForceExpandHeight = false;
            leftLayoutGroup.childForceExpandWidth = true;
            leftLayoutGroup.spacing = 10; // Adjust the spacing between images if needed

            // Set the size of the left panel
            leftPanel.sizeDelta = new Vector2(leftPanelWidth, leftPanel.sizeDelta.y);

            // Adjust the size of the right panel
            rightPanel.sizeDelta = new Vector2(rightPanel.parent.GetComponent<RectTransform>().rect.width - leftPanelWidth, rightPanel.sizeDelta.y);

            foreach (RectTransform child in leftPanel)
            {
                child.sizeDelta = new Vector2(leftPanelWidth, 100f); // Set the size of each RawImage
                Button button = child.gameObject.AddComponent<Button>();
                button.onClick.AddListener(() => SwitchImage(child));
            }

            // Initialize the right panel with the first image if available
            if (leftPanel.childCount > 0)
            {
                selectedImage = leftPanel.GetChild(0).GetComponent<RectTransform>();
                SwitchImage(selectedImage);
            }
        }

        void SwitchImage(RectTransform newImage)
        {
            if (selectedImage != null)
            {
                selectedImage.SetParent(leftPanel);
            }

            newImage.SetParent(rightPanel);
            newImage.sizeDelta = new Vector2(rightPanel.rect.width, rightPanel.rect.height); // Adjust the size of the selected image
            selectedImage = newImage;
        }
    }
}
