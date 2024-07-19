using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class ValueFrom : MonoBehaviour
    {
        private TMP_Text valueTxt;
        [SerializeField] private Transform _userRoosterContainer;
        

        private void Awake()
        {
            valueTxt = GetComponent<TMP_Text>();
        }

        private int usercount = 0;

        private void Update()
        {
            if ( _userRoosterContainer.childCount != usercount )
            {
                usercount = _userRoosterContainer.childCount;
                valueTxt.text = usercount.ToString();
            }
        }
    }
}
