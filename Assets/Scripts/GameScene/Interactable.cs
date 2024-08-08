using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class Interactable : NetworkBehaviour
    {
        public int InteractionCode;
        [SerializeField] protected GameObject _highlightSprite;
        [SerializeField] protected Activity _linkedActivity;
        [SerializeField] protected VoidEventHandlerSO _onInteractionTriggered;

        [Networked] public bool LimitToOne { get; set; }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (!networkObject.HasStateAuthority)
                    return;
            }    

            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                ToggleHighlight(true);
                playerObject.SetInteractionData(InteractionCode, transform.position, this);
                _onInteractionTriggered.RaiseEvent();
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (!networkObject.HasStateAuthority)
                    return;
            }

            if (collision.transform.parent.TryGetComponent<PlayerController>(out var playerObject))
            {
                ToggleHighlight(false);
                playerObject.SetInteractionData(-1);
                _onInteractionTriggered.RaiseEvent();
            }
        }

        public void ToggleHighlight(bool value)
        {
            if (_highlightSprite != null)
            {
                _highlightSprite.SetActive(value);
            }
        }

        public void SetLimit(bool value, int playerNetworkedId)
        {
            if (value)
            {
                LimitToOne = value;
                if (_linkedActivity != null)
                    _linkedActivity.AddPlayer(playerNetworkedId);
            }
            else
            {
                LimitToOne = value;
                if (_linkedActivity != null)
                    _linkedActivity.RemovePlayer(playerNetworkedId);
            }
        }

        public bool GetActivityAvail()
        {
            return _linkedActivity != null;
        }

        public void SendInvite()
        {
            if (_linkedActivity == null)
                return;

            foreach (var player in _linkedActivity.InteractingPlayers)
            {
                SendInvitationToAnotherUserRpc(GetPlayerRefById(player));
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void SendInvitationToAnotherUserRpc([RpcTarget] PlayerRef player)
        {
            if (_linkedActivity.InteractingPlayers.Count > 1)
            {
                _linkedActivity.StartActivity();
            }
        }

        private PlayerRef GetPlayerRefById(int playerId)
        {
            foreach (PlayerRef playerRef in Runner.ActivePlayers)
            {
                if (Runner.GetPlayerObject(playerRef) is NetworkObject playerObject && playerObject.InputAuthority.PlayerId == playerId)
                {
                    return playerRef;
                }
            }
            return PlayerRef.None;
        }
    }
}
