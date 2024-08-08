using UnityEngine;
using AccountManagement;
using Cysharp.Threading.Tasks;
using Fusion;
using DG.Tweening;
using TMPro;

namespace ConnectSphere
{
    public class WhiteboardInteractable : Interactable
    {
        [SerializeField] private BooleanEventHandlerSO _onUiInteracting;
        [SerializeField] private int _whiteboardId;
        [SerializeField] private GameObject _linkedCanvas;
        [SerializeField] private TMP_InputField _inputField;

        [HideInInspector] [Networked] public NetworkString<_16> InputContent { get; set; }
        public bool IsActivated { get; set; }

        private Note _thisNote;
        private bool _isNoteDirty;

        public override void Spawned()
        {
            //InputContent = await GetNoteContentFromDb();
            _inputField.text = $"{InputContent}";
            _inputField.onValueChanged.AddListener(SyncRpc);
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
            //SaveNoteContentToDb(_inputField.text);
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
            var thisNote = await ApiManager.Instance.NoteApi.GetNote(_whiteboardId);
            if (thisNote == null)
            {
                _thisNote = await ApiManager.Instance.NoteApi.CreateNote(_whiteboardId, _inputField.text.Trim());
                if (_thisNote != null)
                {
                    return _thisNote.last_version.content;
                }
                else
                {
                    return string.Empty;
                }
            }
            return thisNote.content;
        }

        private async void SaveNoteContentToDb(string text)
        {
            if (_isNoteDirty)
                await ApiManager.Instance.NoteApi.UpdateNote(_whiteboardId, text);
        }
    }
}
