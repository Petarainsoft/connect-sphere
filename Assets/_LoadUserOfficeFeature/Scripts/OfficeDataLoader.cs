using AccountManagement;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

        [SerializeField]
        private Button _createOfficeButton;

        [SerializeField]
        private string ERROR_LOG = "Something went wrong";

        [SerializeField]
        TextMeshProUGUI _usernameBox;

        [SerializeField]
        MenuManager _menuManager;

        [SerializeField]
        GameObject _officeItemHolder;

        [SerializeField]
        private string _officeResourcePath;

        [SerializeField] 
        private Sprite _defaultTemplate;

        private GridLayoutGroup _layoutHolderOfficeData;
        private List<GameObject> _offices = new List<GameObject>();
        private TMP_InputField _searchField;
        private const int NUMBER_COLUMNS_IN_ROW = 3;
        private const int SECONDS_IN_MINUTE = 60;
        private const int SECONDS_IN_HOUR = 3600;
        private const int SECONDS_IN_DAY = 3600 * 24;

        public int ActiveCount { get; private set; }
        private void OnEnable()
        {
            _ = LoadAllOffices();

            _createOfficeButton.onClick.AddListener(() => { });
        }

        private void OnDisable()
        {
            foreach (var office in _offices)
            {
                RemoveOnOfficeClickEventListener(office);
                Destroy(office);
            }
            _offices.Clear();
            _createOfficeButton.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            _layoutHolderOfficeData = GetComponentInChildren<GridLayoutGroup>();
            _searchField = _searchBar.GetComponentInChildren<TMP_InputField>();

            _searchField.interactable = false;

            Debug.Log(transform.childCount);
        }


        public void Search()
        {
            ActiveCount = 0;
            for (int i = 0; i < _officeSO.Count; i++)
            {
                bool isActive = _officeSO[i].name.Contains(_searchField.text);
                _offices[i].SetActive(isActive);
                if (isActive) ActiveCount += 1;
            }

            SetContentOfficeHeight(ActiveCount);
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
                warningPopup.Data.SetLabelsTexts("Error", ERROR_LOG);
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
            _usernameBox.text = PlayerPrefs.GetString("username").Split("@")[0];

            int count = _officeSO.Count;
            ActiveCount = count;
            SetContentOfficeHeight(count);
            for (int i = 0; i < count; i++)
            {
                GameObject office = Instantiate(_officeItemPrefabs, _officeItemHolder.transform);
                office.name = _officeSO[i].name;
                FillItemData(office, i);
                AddOnOfficeClickEventListener(office);
                _offices.Add(office);
            }
            _searchField.interactable = true;
        }

        private void AddOnOfficeClickEventListener(GameObject office)
        {
            Button officeButton = office.transform.GetOrAddComponent<Button>();
            officeButton.onClick.AddListener(() =>
            {
                _menuManager.OnJoinOfficeEvent(office.name);
                gameObject.SetActive(false);
            });
        }

        private void RemoveOnOfficeClickEventListener(GameObject office)
        {
            Button officeButton = office.GetComponent<Button>();
            officeButton.onClick.RemoveAllListeners();
        }

        private void FillItemData(GameObject office, int i)
        {
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
            Sprite template = Resources.Load<Sprite>(_officeResourcePath + _officeSO[i].resource_url);
            Debug.Log(_officeResourcePath + _officeSO[i].resource_url);
            office.transform.GetChild(2).GetOrAddComponent<Image>().sprite = template ?? _defaultTemplate ;


        }

        private float CalculateHeight(int count)
        {
            float elementHeight = _layoutHolderOfficeData.cellSize.y;
            float spacingTop = _layoutHolderOfficeData.spacing.y;
            float paddingTop = _layoutHolderOfficeData.padding.top;
            Debug.Log(count / NUMBER_COLUMNS_IN_ROW);
            return paddingTop
                + (elementHeight + spacingTop) * (Mathf.CeilToInt((float)count/NUMBER_COLUMNS_IN_ROW));
        }

        public void JoinOffice(string name)
        {
            _ = CreateNewOffice(name);
        }
        public void UpdateLastAccess(string officename, string username) { 
            _= _officeApiHandler.UpdateLastAccess(officename, username);
        }
        private async UniTaskVoid CreateNewOffice(string office_name)
        {
            Utils.ShowLoading();

            var officeResult = await _officeApiHandler.CreateNewOffice(office_name, "/office-1");

            if (officeResult.data == null)
            {
                //handle error
                var warningPopup = UIPopupManager.GetPopup("ActionPopup");
                warningPopup.Data.SetButtonsLabels("Ok");
                warningPopup.Data.SetLabelsTexts("Error", ERROR_LOG);
                warningPopup.Show();
            }
            else
            {
                Debug.Log("Success " + officeResult.data);
            }

            Utils.HideLoading();
        }

        public void Filter(string typeUser)
        {
            ActiveCount = 0;
            for (int i = 0; i < _officeSO.Count; i++) {
                bool isActive = _officeSO[i].type_user.Contains(typeUser) && _officeSO[i].name.Contains(_searchField.text);
                _offices[i].SetActive(isActive);
                if(isActive) ActiveCount += 1;
            }
            SetContentOfficeHeight(ActiveCount);

        }

        private void SetContentOfficeHeight(int count)
        {
            RectTransform rect = _officeItemHolder.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, CalculateHeight(count));
        }

    }
}
