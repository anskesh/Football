using System;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Football.Core
{
    public class Engine
    {
        public static Dictionary<Type, IService> Services { get; private set; } = new Dictionary<Type, IService>();
        public static RuntimeBehaviour Behaviour { get; set; }

        private static ResourceLoader _resourceLoader;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            Behaviour = new GameObject("Runtime", typeof(RuntimeBehaviour)).GetComponent<RuntimeBehaviour>();
            _resourceLoader = new ResourceLoader();
            
            AddService(new InputService());
            AddService(new PoolService());
            AddService(new NetworkService());
            AddService(new BallService());
            AddService(new UIService());
        }

        ~Engine()
        {
            foreach (var service in Services)
                service.Value.Destroy();
        }

        public static T GetConfiguration<T>() where T : Configuration
        {
            return _resourceLoader.GetConfiguration<T>();
        }
        
        public static void AddService<T>(T service) where T : IService
        {
            if (Services.ContainsKey(typeof(T)))
                return;
            
            Services.Add(typeof(T), service);
        }
        
        public static T GetService<T>() where T : IService
        {
            if (!Services.ContainsKey(typeof(T)))
                throw new Exception("Service doesn't exists.");

            return (T) Services[typeof(T)];
        }

        public static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T: Object
        {
            if (!Behaviour)
                throw new Exception("Behaviour doesn't exists.");

            return Object.Instantiate(prefab, position, rotation, parent ? parent : Behaviour.transform);
        }

        public static GameObject CreateObject(string name, Transform parent = null, params Type[] components)
        {
            if (!Behaviour)
                throw new Exception("Behaviour doesn't exists.");

            var gameObject = new GameObject(name ?? "New object", components);
            gameObject.transform.SetParent(parent ? parent : Behaviour.transform);
            
            return gameObject;
        }
    }
}