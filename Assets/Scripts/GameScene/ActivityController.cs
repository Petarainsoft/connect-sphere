using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class ActivityController : NetworkBehaviour
    {
        [Header("Common")]
        [SerializeField] protected Button _buttonQuit;
        [SerializeField] protected BooleanEventHandlerSO _onActivityPanelToggled;

        [Header("Data")]
        public List<int> PlayingUserIds = new List<int>();

        private void Start()
        {
            _buttonQuit.onClick.AddListener(QuitActivity);
        }

        public void FillPlayers(NetworkLinkedList<int> players)
        {
            PlayingUserIds = players.ToList<int>();
        }

        private void QuitActivity()
        {
            foreach (int id in PlayingUserIds)
            {
                CallQuitRpc(GetPlayerRefById(id));
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void CallQuitRpc([RpcTarget] PlayerRef player)
        {
            _onActivityPanelToggled.RaiseEvent(false);
            gameObject.SetActive(false);
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
