using System.Collections;
using Football.Core;
using Mirror;
using Services;
using UnityEngine;
using NetworkBehaviour = Mirror.NetworkBehaviour;

namespace Football
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class Ball : NetworkBehaviour
    {
        [SyncVar] public NetworkIdentity Owner;
        
        [SerializeField] private Rigidbody _rigidbody;

        private bool _canDestroy = false;

        protected override void OnValidate()
        {
            base.OnValidate();
            
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (isServer == false || _canDestroy == false)
                return;
            
            _canDestroy = false;
            NetworkServer.UnSpawn(gameObject);
            Engine.GetService<PoolService>().Return(gameObject);
        }

        [Server]
        public void Move(Vector3 direction, ForceMode mode, float lifeTime)
        {
            gameObject.SetActive(true);
            _rigidbody.AddForce(direction, mode);
            StartCoroutine(Destroy(lifeTime));
        }
        
        private IEnumerator Destroy(float time)
        {
            yield return new WaitForSeconds(time);
            _canDestroy = true;
        }
    }
}