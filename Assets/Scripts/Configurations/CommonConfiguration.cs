using System;
using System.Collections.Generic;
using Football;
using Football.Core;
using UnityEngine;

namespace Configurations
{
    public class CommonConfiguration : Configuration
    {
        public List<ColorSettings> Colors;
        
        [Serializable]
        public class ColorSettings
        {
            public EColor Type;
            public Color Value;
        }
    }
}