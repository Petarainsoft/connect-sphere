using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class VideoLayoutManager : MonoBehaviour
    {
        public RectTransform contentPanel;
        public GameObject rawImagePrefab;
        public int numberOfImages = 10;
        public float imageHeight = 100f;
        public float leftPanelWidth = 200f;
        public Toggle horizontalToggle;
        public Toggle gridToggle;
        public Toggle splitToggle;

        void Start()
        {
            horizontalToggle.onValueChanged.AddListener(delegate { ToggleLayout(); });
            gridToggle.onValueChanged.AddListener(delegate { ToggleLayout(); });
            splitToggle.onValueChanged.AddListener(delegate { ToggleLayout(); });

            InitializeLayout();
        }

        void InitializeLayout()
        {
            for (int i = 0; i < numberOfImages; i++)
            {
                Instantiate(rawImagePrefab, contentPanel);
            }

            ToggleLayout();
        }

        void ToggleLayout()
        {
            ClearLayout();

            if ( horizontalToggle.isOn )
            {
                SetupHorizontalLayout();
            }
            else if ( gridToggle.isOn )
            {
                SetupGridLayout();
            }
            else if ( splitToggle.isOn )
            {
                SetupSplitLayout();
            }
        }

        void ClearLayout()
        {
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }
        }

        void SetupHorizontalLayout()
        {
            contentPanel.GetComponent<HorizontalLayoutGroup>().enabled = true;
            contentPanel.GetComponent<GridLayoutGroup>().enabled = false;
            contentPanel.GetComponent<VerticalLayoutGroup>().enabled = false;

            for (int i = 0; i < numberOfImages; i++)
            {
                GameObject newImage = Instantiate(rawImagePrefab, contentPanel);
                RectTransform rectTransform = newImage.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, imageHeight);
            }

            float totalWidth = numberOfImages*(rawImagePrefab.GetComponent<RectTransform>().sizeDelta.x +
                                               contentPanel.GetComponent<HorizontalLayoutGroup>().spacing);
            contentPanel.sizeDelta = new Vector2(totalWidth, contentPanel.sizeDelta.y);
        }

        void SetupGridLayout()
        {
            contentPanel.GetComponent<HorizontalLayoutGroup>().enabled = false;
            contentPanel.GetComponent<GridLayoutGroup>().enabled = true;
            contentPanel.GetComponent<VerticalLayoutGroup>().enabled = false;

            GridLayoutGroup gridLayoutGroup = contentPanel.GetComponent<GridLayoutGroup>();
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = 2;

            for (int i = 0; i < numberOfImages; i++)
            {
                Instantiate(rawImagePrefab, contentPanel);
            }
        }

        void SetupSplitLayout()
        {
            contentPanel.GetComponent<HorizontalLayoutGroup>().enabled = false;
            contentPanel.GetComponent<GridLayoutGroup>().enabled = false;
            contentPanel.GetComponent<VerticalLayoutGroup>().enabled = false;

            RectTransform leftPanel = new GameObject("LeftPanel", typeof(RectTransform), typeof(VerticalLayoutGroup))
                .GetComponent<RectTransform>();
            leftPanel.SetParent(contentPanel);
            leftPanel.sizeDelta = new Vector2(leftPanelWidth, contentPanel.sizeDelta.y);
            leftPanel.anchorMin = new Vector2(0, 0);
            leftPanel.anchorMax = new Vector2(0, 1);
            leftPanel.pivot = new Vector2(0, 0.5f);

            RectTransform rightPanel =
                new GameObject("RightPanel", typeof(RectTransform)).GetComponent<RectTransform>();
            rightPanel.SetParent(contentPanel);
            rightPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x - leftPanelWidth, contentPanel.sizeDelta.y);
            rightPanel.anchorMin = new Vector2(0, 0);
            rightPanel.anchorMax = new Vector2(1, 1);
            rightPanel.pivot = new Vector2(0, 0.5f);
            rightPanel.anchoredPosition = new Vector2(leftPanelWidth, 0);

            for (int i = 0; i < numberOfImages - 1; i++)
            {
                GameObject newImage = Instantiate(rawImagePrefab, leftPanel);
            }

            GameObject lastImage = Instantiate(rawImagePrefab, rightPanel);
            Button button = lastImage.AddComponent<Button>();
            button.onClick.AddListener(delegate { SwitchImage(leftPanel, rightPanel, button.gameObject); });
        }

        void SwitchImage(RectTransform leftPanel, RectTransform rightPanel, GameObject clickedImage)
        {
            if ( leftPanel.childCount > 0 )
            {
                Transform firstImage = leftPanel.GetChild(0);
                firstImage.SetParent(rightPanel);
                clickedImage.transform.SetParent(leftPanel);
            }
        }
    }
}