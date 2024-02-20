using Football;
using Football.Core;
using UnityEngine;

namespace Configurations
{
    public class BallConfiguration : Configuration
    {
        public Ball BallTemplate;

        [Range(0, 1000)] public float Force = 20;
    }
}