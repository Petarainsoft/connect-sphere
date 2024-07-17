using System;
using UnityEngine;

namespace UI.Chat
{
    public class AddMoreUserPopup : MonoBehaviour
    {
        public event Action OnClose = delegate { };
        public event Action<int> OnAddMoreUser = delegate { }; 
    }
}