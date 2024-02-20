using Mirror;
using UnityEngine;

namespace Football
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class Ball : NetworkBehaviour
    {
        
        [SyncVar] public NetworkIdentity Owner;
        
        [SerializeField] private Rigidbody _rigidbody;

        protected override void OnValidate()
        {
            base.OnValidate();
            
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void AddForce(Vector3 direction, ForceMode mode)
        {
            _rigidbody.AddForce(direction, mode);
        }
    }
}