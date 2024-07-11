using UnityEngine;
using DG.Tweening;

namespace ConnectSphere
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 rotation;
        public float speed;

        // Start is called before the first frame update
        void Start()
        {
            transform.DORotate(rotation, speed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetRelative().SetEase(Ease.Linear);
        }
    }
}
