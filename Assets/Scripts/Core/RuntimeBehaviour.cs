using UnityEngine;

namespace Football.Core
{
    public class RuntimeBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}