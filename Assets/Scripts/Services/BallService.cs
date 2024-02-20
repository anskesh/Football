using System.Collections.Generic;
using System.Linq;
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
        
        private Transform _ballContainer;
        private List<Ball> _balls = new List<Ball>();
        
        public BallService()
        {
            _ballContainer = Engine.CreateObject("BallsContainer").transform;
            Configuration = Engine.GetConfiguration<BallConfiguration>();
        }

        public void SpawnBall(NetworkIdentity owner, Vector3 position, Vector3 direction)
        {
            var ball = GetOrCreateBall(position);
            ball.Owner = owner;
            ball.AddForce(direction * Configuration.Force, ForceMode.Impulse);
        }

        private Ball GetOrCreateBall(Vector3 position)
        {
            Ball ball = null;
            
            if (_balls.All(x => x.gameObject.activeSelf))
            {
                ball = Engine.Instantiate(Configuration.BallTemplate, position, Quaternion.identity,
                    _ballContainer);
                NetworkServer.Spawn(ball.gameObject);
                return ball;
            }

            ball = _balls.First(x => x.gameObject.activeSelf == false);
            ball.transform.position = position;
            ball.gameObject.SetActive(true);
            
            return ball;
        }
        
        public void Initialize()
        {
        }

        public void Destroy()
        {
        }
    }
}