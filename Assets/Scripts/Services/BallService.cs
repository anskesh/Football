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
        private float _lifeTime;
        
        public BallService()
        {
            Configuration = Engine.GetConfiguration<BallConfiguration>();
            _poolService = Engine.GetService<PoolService>();
            _lifeTime = Configuration.MaxLifeTime;

            _poolService.OnCountChanged += OnCountChanged;
            Engine.GetService<NetworkService>().NetworkManager.OnClientConnected += InitializeBalls;
        }

        private void InitializeBalls()
        {
            _poolService.CreateNew(5, Configuration.BallTemplate.gameObject);
            Engine.GetService<NetworkService>().NetworkManager.OnClientConnected -= InitializeBalls;
        }

        public void SpawnBall(NetworkIdentity owner, Vector3 position, Vector3 direction)
        {
            var ball = _poolService.Get(Configuration.BallTemplate.gameObject, position, Quaternion.identity).GetComponent<Ball>();
            NetworkServer.Spawn(ball.gameObject);
            ball.Owner = owner;
            ball.Move(direction * Configuration.MinForce, ForceMode.Impulse, _lifeTime);
        }

        private void OnCountChanged(string name, PoolService.PoolObjectCount count)
        {
            if (!Configuration.BallTemplate.name.Equals(name))
                return;

            _lifeTime = CalculateBallLifeTime(count);
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

        public void Initialize()
        {
        }

        public void Destroy()
        {
            _poolService.OnCountChanged -= OnCountChanged;
        }
    }
}