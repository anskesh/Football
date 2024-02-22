using System;
using System.Collections.Generic;
using Configurations;
using Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Football.Core
{
    public class Engine
    {
        public static RuntimeBehaviour Behaviour { get; set; }

        private static Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            Behaviour = new GameObject("Runtime", typeof(RuntimeBehaviour)).GetComponent<RuntimeBehaviour>();
            
            AddService(new InputService(GetConfiguration<InputConfiguration>()));
            AddService(new UIService(GetConfiguration<UIConfiguration>()));
            AddService(new NetworkService(GetConfiguration<NetworkConfiguration>(), GetService<UIService>()));
            AddService(new PoolService(GetService<NetworkService>()));
            AddService(new BallService(GetConfiguration<BallConfiguration>(), GetService<PoolService>(), GetService<NetworkService>()));
        }

        ~Engine()
        {
            foreach (var service in _services)
                service.Value.Destroy();
        }

        public static T GetConfiguration<T>() where T : Configuration
        {
            return ResourceLoader.GetConfiguration<T>();
        }
        
        public static void AddService<T>(T service) where T : IService
        {
            if (_services.ContainsKey(typeof(T)))
                return;
            
            _services.Add(typeof(T), service);
        }
        
        public static T GetService<T>() where T : IService
        {
            if (!_services.ContainsKey(typeof(T)))
                throw new Exception("Service doesn't exists.");

            return (T) _services[typeof(T)];
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

        public static void Destroy(GameObject gameObject, float time = 0)
        {
            Object.Destroy(gameObject, time);
        }
    }
}