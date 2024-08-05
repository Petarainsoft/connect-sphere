using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class SearchingHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public bool _isInit = false;
    [SerializeField] private GameObject _content;
    private TMP_InputField _inputField;
    private List<TextMeshProUGUI> _items = new();
    void Start()
    {
        _inputField = GetComponent<TMP_InputField>();


    }

    // Update is called once per frame
    public void OnSearch()
    {
        if (!_isInit)
        {
            int num = _content.transform.childCount;
            Debug.Log(num);

            for (int i = 0; i < num; i++)
            {
                _items.Add(_content.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>());
                Debug.Log(_items[i].text);

            }
        }

        string searchText = _inputField.text;
        Debug.Log(searchText);
        foreach (var item in _items)
        {
            if (!item.text.Contains(searchText))
            {
                item.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                item.transform.parent.gameObject.SetActive(true);

            }
        }

    }


}

