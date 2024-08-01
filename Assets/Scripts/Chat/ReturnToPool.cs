using Chat;
using UnityEngine;
using UnityEngine.Pool;

namespace ConnectSphere
{
    // This component returns the particle system to the pool when the OnParticleSystemStopped event is received.
    [RequireComponent(typeof(VideoSingleConnection))]
    public class ReturnToPool : MonoBehaviour
    {
        public VideoSingleConnection system;
        public IObjectPool<VideoSingleConnection> pool;

        void Start()
        {
            system = GetComponent<VideoSingleConnection>();
            // var main = system.main;
            // main.stopAction = ParticleSystemStopAction.Callback;
        }

        public void Return()
        {
            // Return to the pool
            pool.Release(system);
        }
    }
}
