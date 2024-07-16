using DG.Tweening;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class BubbleChat : NetworkBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _representImage;
        [SerializeField] private List<Sprite> _sprites;

        private float _showDuration = 2.15f;
        private float _fadeDuration = 0.15f;
        private float _elapsedTime = 0;
        private bool _isShowing = false;

        private void Update()
        {
            if (_isShowing)
            {
                _elapsedTime += Time.deltaTime;
            }
        }

        public IEnumerator Show(int index)
        {
            _representImage.sprite = _sprites[index];
            _canvasGroup.DOFade(1, _fadeDuration);
            _elapsedTime = 0;
            _isShowing = true;
            while (_elapsedTime < _showDuration)
            {
                yield return null;
            }
            Hide();
        }

        public void Hide()
        {
            _isShowing = false;
            _canvasGroup.DOFade(0, _fadeDuration);
        }
    }
}
