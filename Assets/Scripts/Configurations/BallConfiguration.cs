using Football;
using Football.Core;

namespace Configurations
{
    public class BallConfiguration : Configuration
    {
        public Ball BallTemplate;
        
        public float MaxLifeTime = 20f;
        public float MinLifeTime = 5f;
        public int MaxBallCount = 20;

        public float MinForce = 10;
        public float MaxForce = 50;
    }
}