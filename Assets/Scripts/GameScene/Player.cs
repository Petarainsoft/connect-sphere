using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textPlayerName;

        private PlayerInput _input;

        private void Start()
        {
            _input = GetComponent<PlayerInput>();
        }

        public void SetName(string name)
        {
            _textPlayerName.text = name;
        }
    }
}
