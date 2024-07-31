using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fusion;
using TMPro;

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

        private PlayerController _playerController;

        public override void Spawned()
        {
            _inputField.text = $"{InputContent}";
        }

        public void ActivateLocalCanvas(PlayerController playerController)
        {
            if (!IsActivated)
            {
                IsActivated = true;
                _linkedCanvas.SetActive(true);
                _linkedCanvas.GetComponent<CanvasGroup>().DOFade(1, 0.15f);
                _playerController = playerController;
            }
        }

        public void TurnOffSticker()
        {
            InputContent = _inputField.text;
            SyncRpc(_inputField.text);
            IsActivated = false;
            _linkedCanvas.GetComponent<CanvasGroup>().DOFade(0, 0.15f).OnComplete(() => _linkedCanvas.SetActive(false));
            _onUiInteracting.RaiseEvent(false);
            _playerController = null;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void SyncRpc(string text)
        {
            _inputField.text = text;
            InputContent = text;
        }
    }
}
