using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class GridLayout : MonoBehaviour
    {
        public RectTransform contentTransform;
        public float cellSize = 100f; // Set the desired size of each cell

        void Start()
        {
            GridLayoutGroup layoutGroup = contentTransform.gameObject.AddComponent<GridLayoutGroup>();
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 2; // Set the number of columns
            layoutGroup.cellSize = new Vector2(cellSize, cellSize); // Set the size of each cell
            layoutGroup.spacing = new Vector2(10, 10); // Adjust the spacing between cells if needed

            // Adjust the size of the content transform to fit the grid
            int rowCount = Mathf.CeilToInt(contentTransform.childCount / 2.0f);
            contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, rowCount * (cellSize + layoutGroup.spacing.y));
        }
    }
}
