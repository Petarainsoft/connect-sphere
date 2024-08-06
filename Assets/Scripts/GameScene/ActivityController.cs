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
        [Header("UI")]
        [SerializeField] private Button _buttonQuit;
        [SerializeField] private BooleanEventHandlerSO _onActivityPanelToggled;

        [Header("Data")]
        public List<int> OngoingPlayers = new List<int>();

        private void Start()
        {
            _buttonQuit.onClick.AddListener(QuitActivity);
        }

        public void FillPlayers(NetworkLinkedList<int> players)
        {
            OngoingPlayers = players.ToList<int>();
        }

        private void QuitActivity()
        {
            foreach (int id in OngoingPlayers)
            {
                CallQuitRpc(GetPlayerRefById(id));
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void CallQuitRpc([RpcTarget] PlayerRef player)
        {
            Debug.Log("QUIT GAME " + player.PlayerId);
            _onActivityPanelToggled.RaiseEvent(false);
            Destroy(gameObject, 0.1f);
        }

        private PlayerRef GetPlayerRefById(int playerId)
        {
            Debug.Log($"{Runner}");
            Debug.Log($"{Runner.ActivePlayers}");
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
