using Football.Core;
using Mirror;
using Services;
using UnityEngine;
using NetworkBehaviour = Mirror.NetworkBehaviour;

namespace Football
{
    public class Cannon : NetworkBehaviour
    {
        [SerializeField] private Transform _barrel;
        
        private Quaternion _defaultRotation;
        
        private BallService _ballService;
        private InputService _inputService;

        private void Awake()
        {
            _inputService = Engine.GetService<InputService>();
            _ballService = Engine.GetService<BallService>();

            _defaultRotation = transform.rotation;
        }

        private void Update()
        {
            if (_inputService == null || isOwned == false)
                return;
            
            transform.LookAt(_inputService.GetMousePosition());
            
            if (_inputService.GetPressedFireButton())
                CmdSpawnBall(netIdentity, _barrel.position, _barrel.forward);
        }

        public void ResetRotation()
        {
            transform.rotation = _defaultRotation;
        }

        [Command]
        private void CmdSpawnBall(NetworkIdentity owner, Vector3 position, Vector3 direction)
        {
            _ballService.SpawnBall(owner, position, direction);
        }
    }
}