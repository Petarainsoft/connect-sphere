using Fusion;
using UnityEngine;

namespace ConnectSphere
{
    public class RaycastAttack : NetworkBehaviour
    {
        public float Damage = 10;

        public PlayerMovement PlayerMovement;

        void Update()
        {
            if (HasStateAuthority == false)
            {
                return;
            }
            Ray ray = PlayerMovement.Camera.ScreenPointToRay(Input.mousePosition);
            ray.origin += PlayerMovement.Camera.transform.forward;

            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.DrawRay(ray.origin, ray.direction * 5, Color.red, 1f);

                if (Runner.GetPhysicsScene().Raycast(ray.origin, ray.direction, out var hit))
                {
                    if (hit.transform.TryGetComponent<Health>(out var health))
                    {
                        health.DealDamageRpc(Damage);
                    }
                }
            }
        }
    }
}
