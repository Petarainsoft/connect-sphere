using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class VideoLayoutManager : MonoBehaviour
    {
        public LayoutMode _currentMode = LayoutMode.HorizontalTop;
        public LayoutMode _prvMode = LayoutMode.None;

        public UIToggle _horizontalToggle;
        public UIToggle _gridToggle;
        public UIToggle _leftRightToggle;

        public RectTransform _gridContainer;
        public RectTransform _horizontalContainer;
        public RectTransform _leftRightContainer;
        public RectTransform _focusImageContainer;
        

        private void Start()
        {
            _horizontalToggle.Toggle.onValueChanged.AddListener((isOn) => { if (isOn) _currentMode = LayoutMode.HorizontalTop; });
            _gridToggle.Toggle.onValueChanged.AddListener((isOn) => { if (isOn) _currentMode = LayoutMode.CenterGrid; });
            _leftRightToggle.Toggle.onValueChanged.AddListener((isOn) => { if (isOn) _currentMode = LayoutMode.LeftRight; });
        }

        private void Update()
        {
            if ( _currentMode == _prvMode ) return;
            ChangeLayout(_prvMode, _currentMode);
            
            
            
            _prvMode = _currentMode;
        }

        private void ChangeLayout(LayoutMode prevMode, LayoutMode layoutMode)
        {
            RectTransform sourceContainer = null;
            switch (prevMode)
            {
                case LayoutMode.None:
                    return;
                case LayoutMode.HorizontalTop:
                    sourceContainer = _horizontalContainer;
                    break;
                case LayoutMode.CenterGrid:
                    sourceContainer = _gridContainer;
                    break;
                case LayoutMode.LeftRight:
                    sourceContainer = _leftRightContainer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(prevMode), prevMode, null);
            }
            
            RectTransform targetContainer = null;
            switch (layoutMode)
            {
                case LayoutMode.None:
                    break;
                case LayoutMode.HorizontalTop:
                    targetContainer = _horizontalContainer;
                    break;
                case LayoutMode.CenterGrid:
                    targetContainer = _gridContainer;
                    break;
                case LayoutMode.LeftRight:
                    targetContainer = _leftRightContainer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layoutMode), layoutMode, null);
            }
            
            var childCount = sourceContainer.childCount;
        
            for (int i = 0; i < childCount; i++)
            {
                Transform child = sourceContainer.GetChild(0); // Lấy đối tượng con đầu tiên
                child.SetParent(targetContainer); // Đặt đối tượng con vào đối tượng cha mới
            }
        }
    }

    public enum LayoutMode
    {
        None,
        HorizontalTop,
        CenterGrid,
        LeftRight
    }
}