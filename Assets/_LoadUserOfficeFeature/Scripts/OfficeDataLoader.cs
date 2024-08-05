using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class OfficeDataLoader : MonoBehaviour
    {
        [SerializeField] OfficeSO _officeSO;
        [SerializeField]
        private GameObject _itemPrefabs;
        private GridLayoutGroup layout;

        private List<GameObject> items = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            layout = GetComponent<GridLayoutGroup>();
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, CalculateHeight(_officeSO.Count));

            FillData();
            Debug.Log(transform.childCount);
        }

        private void FillData()
        {
            int count = _officeSO.Count;
            for (int i = 0; i < count; i++)
            {

                GameObject item = Instantiate(_itemPrefabs, transform);
                Debug.Log(item.name);

                FillItemData(item, i);
                items.Add(item);

            }
        }

        private void FillItemData(GameObject item, int i)
        {
            Debug.Log(item.transform.GetComponentInChildren<TextMeshProUGUI>().text);
            item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _officeSO[i].name;
            int seconds = _officeSO[i].GetDateDiff();
            if (_officeSO[i].GetDateDiff() < 60)
            {
                item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Last minute";

            }
            else if (seconds < 3600)
            {
                item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = seconds % 60 + "-minute ago";

            }

            else if (seconds < 3600 * 24)
            {
                item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = seconds % 3600 + "-hour ago";
            }
            else
            {
                item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = seconds % (3600 * 24) + "-day ago";

            }
        }

        private float CalculateHeight(int count)
        {
            float elementHeight = layout.cellSize.y;
            float spacingTop = layout.spacing.y;

            return elementHeight * (count / 3) + spacingTop * count - spacingTop;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
