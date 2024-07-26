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
        [SerializeField] private GameObject _linkedCanvas;
        [SerializeField] private TMP_InputField _inputField;

        [Networked] public string InputContent { get; set; }
        public bool IsActivated { get; set; }

        private PlayerController _playerController;

        public override void Spawned()
        {
            _inputField.text = InputContent;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            base.OnTriggerExit2D(collision);
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
            Debug.Log("DO SOMETHING");
            InputContent = _inputField.text;
            SyncronizeInputFieldRpc(Runner.LocalPlayer);

            IsActivated = false;
            _linkedCanvas.GetComponent<CanvasGroup>().DOFade(0, 0.15f).OnComplete(() => _linkedCanvas.SetActive(false));
            _playerController.SetInteractionStatus(false);
            _playerController = null;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void SyncronizeInputFieldRpc(PlayerRef player)
        {
            if (Runner.LocalPlayer != player)
            {
                _inputField.text = InputContent;
            }
        }
    }
}
