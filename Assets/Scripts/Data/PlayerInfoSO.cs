using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    [CreateAssetMenu(fileName = "PlayerInfoSO", menuName = "Data/Player Info SO")]
    public class PlayerInfoSO : ScriptableObject
    {
        public string PlayerName;
        public int AvatarIndex;
        public NetworkRole Role;
        public string RoomName;
    }

    public enum NetworkRole
    {
        Server = 0,
        Client = 1,
        Host = 2
    }
}
