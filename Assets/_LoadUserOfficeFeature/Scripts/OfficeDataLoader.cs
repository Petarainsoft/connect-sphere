using System.Collections.Generic;
using AccountManagement;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class OfficeDataLoader : MonoBehaviour
    {
        [SerializeField]
        OfficeSO _officeSO;

        [SerializeField]
        OfficeApiHandler _officeApiHandler;

        [SerializeField]
        private GameObject _officeItemPrefabs;

        [SerializeField]
        private GameObject _searchBar;
        private GridLayoutGroup _layoutHolderOfficeData;
        private List<GameObject> _offices = new List<GameObject>();
        private TMP_InputField _searchField;
        private const int NUMBER_COLUMNS_IN_ROW = 3;
        private const int SECONDS_IN_MINUTE = 60;
        private const int SECONDS_IN_HOUR = 3600;
        private const int SECONDS_IN_DAY = 3600 * 24;

        private void Awake()
        {
            _ = LoadAllOffices();
        }

        private void Start()
        {
            base.OnEnter();
            _layoutHolderOfficeData = GetComponent<GridLayoutGroup>();
            _searchField = _searchBar.GetComponentInChildren<TMP_InputField>();

            _searchField.interactable = false;

            Debug.Log(transform.childCount);
        }

        public void Search()
        {
            for (int i = 0; i < _officeSO.Count; i++)
            {
                _offices[i].SetActive(_officeSO[i].name.Contains(_searchField.text));
            }
        }

        private async UniTaskVoid LoadAllOffices()
        {
            Utils.ShowLoading();

            var officeResult = await _officeApiHandler.LoadAllOffices();

            if (officeResult.data == null)
            {
                //handle error
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Message", officeResult.message);
                warningPopup.Show();
            }
            else
            {
                _officeSO.ClearAllOffice();
                foreach (var office in officeResult.data)
                {
                    _officeSO.AddOffice(office);
                }
                Debug.Log("Success " + _officeSO.Count);
                FillData();
            }

            Utils.HideLoading();
        }

        private void FillData()
        {
            int count = _officeSO.Count;
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, CalculateHeight(count));
            for (int i = 0; i < count; i++)
            {
                GameObject office = Instantiate(_officeItemPrefabs, transform);
                Debug.Log(office.name);

                FillItemData(office, i);
                _offices.Add(office);
            }
            _searchField.interactable = true;
        }

        private void FillItemData(GameObject office, int i)
        {
            Debug.Log(office.transform.GetComponentInChildren<TextMeshProUGUI>().text);
            office.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _officeSO[i].name;
            int seconds = _officeSO[i].GetDateDiff();
            if (_officeSO[i].GetDateDiff() < SECONDS_IN_MINUTE)
            {
                office.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Last minute";
            }
            else if (seconds < SECONDS_IN_HOUR)
            {
                office.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    seconds / SECONDS_IN_MINUTE + "-minute ago";
            }
            else if (seconds < SECONDS_IN_DAY)
            {
                office.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    seconds / SECONDS_IN_HOUR + "-hour ago";
            }
            else
            {
                office.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    seconds / SECONDS_IN_DAY + "-day ago";
            }
        }

        private float CalculateHeight(int count)
        {
            float elementHeight = _layoutHolderOfficeData.cellSize.y;
            float spacingTop = _layoutHolderOfficeData.spacing.y;
            float paddingTop = _layoutHolderOfficeData.padding.top;
            Debug.Log(count / NUMBER_COLUMNS_IN_ROW);
            return paddingTop
                + (elementHeight + spacingTop) * ((count / NUMBER_COLUMNS_IN_ROW) + 1);
        }
    }
}
