using UnityEngine;
using DG.Tweening;
using Fusion;
using TMPro;
using AccountManagement;
using Cysharp.Threading.Tasks;

namespace ConnectSphere
{
    public class StickerBoardInteractable : Interactable
    {
        [SerializeField] private BooleanEventHandlerSO _onUiInteracting;
        [SerializeField] private int _stickyNoteId;
        [SerializeField] private GameObject _linkedCanvas;
        [SerializeField] private TMP_InputField _inputField;

        [Networked] public NetworkString<_16> InputContent { get; set; }
        public bool IsActivated { get; set; }

        private Note _thisNote;
        private bool _isNoteDirty;

        public override async void Spawned()
        {
            InputContent = await GetNoteContentFromDb();
            _inputField.text = $"{InputContent}";
        }

        private void Start()
        {
            _inputField.onValueChanged.AddListener(SetDirtyInput);
        }

        public void ActivateLocalCanvas()
        {
            if (!IsActivated)
            {
                IsActivated = true;
                _linkedCanvas.SetActive(true);
                _linkedCanvas.GetComponent<CanvasGroup>().DOFade(1, 0.15f);
            }
        }

        public void TurnOffSticker()
        {
            InputContent = _inputField.text;
            SyncRpc(_inputField.text);
            SaveNoteContentToDb(_inputField.text);
            IsActivated = false;
            _linkedCanvas.GetComponent<CanvasGroup>().DOFade(0, 0.15f).OnComplete(() => _linkedCanvas.SetActive(false));
            _onUiInteracting.RaiseEvent(false);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void SyncRpc(string text)
        {
            _inputField.text = text;
            InputContent = text;
        }

        private void SetDirtyInput(string text)
        {
            _isNoteDirty = true;
        }

        private async UniTask<string> GetNoteContentFromDb()
        {
            var thisNote = await ApiManager.Instance.NoteApi.GetNote(_stickyNoteId);
            if (thisNote == null)
            {
                _thisNote = await ApiManager.Instance.NoteApi.CreateNote(_stickyNoteId, _inputField.text.Trim());
                return _thisNote.last_version.content;
            }
            return thisNote.content;
        }

        private async void SaveNoteContentToDb(string text)
        {
            if (_isNoteDirty)
                await ApiManager.Instance.NoteApi.UpdateNote(_stickyNoteId, text);
        }
    }
}
