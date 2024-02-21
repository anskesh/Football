using Football.Core;
using Mirror;
using Services;
using UI;
using UnityEngine;
using NetworkBehaviour = Mirror.NetworkBehaviour;

namespace Football
{
    public class Cannon : NetworkBehaviour
    {
        [SerializeField] private Transform _barrel;
        [SerializeField] private float _maxPressingTime = 2f;
        
        private Quaternion _defaultRotation;
        
        private BallService _ballService;
        private InputService _inputService;
        private InGameUI _ui;

        private float _pressingTime = 0;

        private void Awake()
        {
            _inputService = Engine.GetService<InputService>();
            _ballService = Engine.GetService<BallService>();
            _ui = Engine.GetService<UIService>().GetUI<InGameUI>();
            _ui.UpdateTime(0);

            _defaultRotation = transform.rotation;
        }

        private void Update()
        {
            if (_inputService == null || isOwned == false)
                return;
            
            transform.LookAt(_inputService.GetMousePosition());
            
            if (_inputService.IsFireButtonUp())
            {
                CmdSpawnBall(netIdentity, _barrel.position, _barrel.forward, _pressingTime / _maxPressingTime);
                _pressingTime = 0;
                UpdateTime();
            }

            if (_inputService.IsPressedFireButton())
            {
                _pressingTime += Time.deltaTime;
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            var percent = _pressingTime / _maxPressingTime;
            _ui.UpdateTime(percent);
        }

        public void ResetRotation()
        {
            transform.rotation = _defaultRotation;
        }

        [Command]
        private void CmdSpawnBall(NetworkIdentity owner, Vector3 position, Vector3 direction, float forceMultiplier)
        {
            _ballService.SpawnBall(owner, position, direction, forceMultiplier);
        }
    }
}