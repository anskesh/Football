using System;
using Configurations;
using Football;
using Football.Core;
using Mirror;
using UnityEngine;

namespace Services
{
    public class BallService : IService<BallConfiguration>
    {
        public BallConfiguration Configuration { get; set; }
        
        private PoolService _poolService;
        private NetworkService _networkService;
        private float _lifeTime;
        
        public BallService(BallConfiguration configuration, PoolService poolService, NetworkService networkService)
        {
            Configuration = configuration;
            
            _poolService = poolService;
            _networkService = networkService;
            _lifeTime = Configuration.MaxLifeTime;

            _poolService.OnCountChanged += OnCountChanged;
            _networkService.ClientConnectedEvent += InitializeBalls;
        }

        private void InitializeBalls()
        {
            var prefab = Configuration.BallTemplate.gameObject;
            
            if (!_poolService.IsCreated(prefab))
                _poolService.CreateNew(5, prefab);
            else
                _poolService.RegisterPrefabs(prefab);
        }

        public void SpawnBall(NetworkIdentity owner, Vector3 position, Vector3 direction, float forceMultiplier)
        {
            var ball = _poolService.Get(Configuration.BallTemplate.gameObject, position, Quaternion.identity).GetComponent<Ball>();
            NetworkServer.Spawn(ball.gameObject);
            ball.Owner = owner;
            ball.Move(CalculateForce(direction, forceMultiplier), ForceMode.Impulse, _lifeTime);
        }

        private Vector3 CalculateForce(Vector3 direction, float forceMultiplier)
        {
            var force = Configuration.MinForce + (Configuration.MaxForce - Configuration.MinForce) * forceMultiplier;
            force = Math.Clamp(force, Configuration.MinForce, Configuration.MaxForce);
            
            return direction * force + new Vector3(0, 3f, 0);
        }
 
        private float CalculateBallLifeTime(PoolService.PoolObjectCount count)
        {
            var currentCount = count.Active;
            var maxCount = Configuration.MaxBallCount;
            var maxLifeTime = Configuration.MaxLifeTime;
            var minLifeTime = Configuration.MinLifeTime;

            var lifeTime = maxLifeTime;
            
            if (currentCount > maxCount)
                lifeTime += (maxCount / (float) currentCount);

            lifeTime = Mathf.Clamp(lifeTime, minLifeTime, maxLifeTime);
            return lifeTime;
        }
        
        private void OnCountChanged(string name, PoolService.PoolObjectCount count)
        {
            if (!Configuration.BallTemplate.name.Equals(name))
                return;

            _lifeTime = CalculateBallLifeTime(count);
        }

        public void Initialize()
        {
        }

        public void Destroy()
        {
            _poolService.OnCountChanged -= OnCountChanged;
            _networkService.ClientConnectedEvent -= InitializeBalls;
        }
    }
}