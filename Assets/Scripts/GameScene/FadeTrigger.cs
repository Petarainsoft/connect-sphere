using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

namespace ConnectSphere
{
    public class FadeTrigger : MonoBehaviour
    {
        public Transform FadeZone;
        public Light2D ZoneLight;
        public Light2D GlobalLight;

        private float focusIntensity = 0.6f;
        private float offIntensity = 0f;
        private float duration = 0.2f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (FadeZone != null)
            {
                foreach (Transform child in FadeZone)
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
            if (FadeZone != null)
            {
                foreach (Transform child in FadeZone)
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
