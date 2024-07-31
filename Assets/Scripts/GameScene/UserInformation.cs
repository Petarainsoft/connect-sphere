using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class UserInformation : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _imageChosenAvatar;
        [SerializeField] private TMP_Text _textLongName;
        [SerializeField] private TMP_InputField _inputBiography;

        [Header("Children Components")]
        [SerializeField] private Transform _avatarsContainer;
        [SerializeField] private Button _buttonCloseAvatarPicker;
        [SerializeField] private Button _buttonCloseTitlePicker;
        [SerializeField] private TMP_Dropdown _dropdownTitles;

        [Header("References")]
        [SerializeField] private GameObject _avatarPickerPanel;
        [SerializeField] private GameObject _titlePickerPanel;

        [Header("Data")]
        [SerializeField] private PlayerInfoSO _playerInfo;
        [SerializeField] private List<Sprite> _avatarSprites = new List<Sprite>();

        [Header("Events")]
        [SerializeField] private IntegerEventHandlerSO _onAvatarImageClicked;

        private int _selectedAvatarIndex = 0;
        private string _selectedTitle;

        private void Awake()
        {
            _buttonCloseAvatarPicker.onClick.AddListener(OnAvatarPickerClosed);
            _buttonCloseTitlePicker.onClick.AddListener(OnTitlePickerClosed);
            _inputBiography.onDeselect.AddListener(SaveBiography);
        }

        private void OnEnable()
        {
            _onAvatarImageClicked.OnEventRaised += HandleSelectedAvatar;
        }

        private void OnDisable()
        {
            _onAvatarImageClicked.OnEventRaised -= HandleSelectedAvatar;
        }

        private void Start()
        {
            if (_playerInfo == null)
                return;

            // name & bio
            _textLongName.text = $"{_playerInfo.PlayerName} {_playerInfo.Title}".Trim();
            _inputBiography.text = _playerInfo.Biography;

            // avatar
            _selectedAvatarIndex = _playerInfo.AvatarIndex;
            _imageChosenAvatar.sprite = _avatarSprites[_selectedAvatarIndex];
            HandleSelectedAvatar(_selectedAvatarIndex);

            // title
            for (int i = 0; i < TitlesData.SpecialTitles.Count; i++)
            {
                _dropdownTitles.options.Add(new TMP_Dropdown.OptionData(TitlesData.SpecialTitles[i]));
                if (TitlesData.SpecialTitles[i] == _playerInfo.Title)
                {
                    _dropdownTitles.value = i;
                }
            }
        }

        public void OnEditAvatarClicked()
        {
            _avatarPickerPanel.SetActive(true);
        }

        public void OnEditTitleClicked()
        {
            _titlePickerPanel.SetActive(true);
        }

        private void HandleSelectedAvatar(int index)
        {
            foreach (Transform child in _avatarsContainer)
            {
                if (child.GetSiblingIndex() == index)
                {
                    child.GetComponent<Image>().enabled = true;
                    _selectedAvatarIndex = index;
                }
                else
                {
                    child.GetComponent<Image>().enabled = false;
                }
            }
        }

        private void OnAvatarPickerClosed()
        {
            _avatarPickerPanel.SetActive(false);
            SaveAvatar();
        }

        private void OnTitlePickerClosed()
        {
            _titlePickerPanel.SetActive(false);
            SaveTitle();
        }

        // TODO: USE APIs TO SAVE PLAYERINFO TO DATABASE

        private void SaveAvatar()
        {
            _playerInfo.AvatarIndex = _selectedAvatarIndex;
            _imageChosenAvatar.sprite = _avatarSprites[_selectedAvatarIndex];
        }

        private void SaveTitle()
        {
            _selectedTitle = _dropdownTitles.captionText.text != "None" ? _dropdownTitles.captionText.text : string.Empty;
            _playerInfo.Title = _selectedTitle;
            _textLongName.text = $"{_playerInfo.PlayerName} {_selectedTitle}".Trim();
        }

        private void SaveBiography(string text)
        {
            _playerInfo.Biography = _inputBiography.text.Trim();
        }
    }
}
