using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    enum PlayerButtons
    {
        Fire = 0,
    }

    public struct PlayerInput : INetworkInput
    {
        public float HorizontalInput;
        public float VerticalInput;
        public NetworkButtons Buttons;
    }
}