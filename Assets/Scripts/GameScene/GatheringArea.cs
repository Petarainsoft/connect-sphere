using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
using System;
using Fusion;
using System.Collections.Generic;

namespace ConnectSphere
{
    public class GatheringArea : MonoBehaviour
    {
        public Transform FadeTexture;
        public Light2D ZoneLight;
        public Light2D GlobalLight;

        [SerializeField] private int areaId;

        private float focusIntensity = 0.6f;
        private float offIntensity = 0f;
        private float duration = 0.2f;

        public static Action<int> OnPlayerEnteredArea;
        public static Action<int> OnPlayerExitArea;

        private List<NetworkObject> _playersInThisArea = new List<NetworkObject>();


        public List<NetworkObject> PlayersInArea => _playersInThisArea;

        public int AreaId => areaId;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var playerObject))
            {
                OnPlayerEnteredArea?.Invoke(areaId);
                _playersInThisArea.Add(playerObject);
                
                if (!playerObject.HasStateAuthority)
                    return;
            }
            else
            {
                return;
            }

            if (FadeTexture != null)
            {
                foreach (Transform child in FadeTexture)
                {
                    Fade(child.GetComponent<SpriteRenderer>(), 0, duration);
                }
            }

            if (ZoneLight != null)
            {
                FadeLight(ZoneLight, focusIntensity, duration);
                FadeLight(GlobalLight, focusIntensity - 0.1f, duration);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<NetworkObject>(out var playerObject))
            {
                OnPlayerExitArea?.Invoke(areaId);
                _playersInThisArea.Remove(playerObject);
                
                if (!playerObject.HasStateAuthority)
                    return;
            }
            else
            {
                return;
            }
            
            if (FadeTexture != null)
            {
                foreach (Transform child in FadeTexture)
                {
                    Fade(child.GetComponent<SpriteRenderer>(), 1, duration);
                }
            }

            if (ZoneLight != null)
            {
                FadeLight(ZoneLight, offIntensity, duration);
                FadeLight(GlobalLight, 1, duration);
            }
        }

        private void Fade(SpriteRenderer spriteRenderer, float endValue, float duration)
        {
            spriteRenderer.DOFade(endValue, duration);
        }

        private void FadeLight(Light2D light, float endValue, float duration)
        {
            DOTween.To(() => light.intensity, x => light.intensity = x, endValue, duration);
        }
    }
}
