using System;
using System.Collections.Generic;
using Football.Core;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Services
{
    public class PoolService : IService
    {
        public Action<string, PoolObjectCount> OnCountChanged;
        
        private Dictionary<string, PoolObjectCount> _poolObjectCounts = new Dictionary<string, PoolObjectCount>();
        private Dictionary<string, Stack<GameObject>> _cached = new Dictionary<string, Stack<GameObject>>();
        
        private Transform _container;
        private NetworkService _networkService;

        public PoolService(NetworkService networkService)
        {
            _container = Engine.CreateObject("Pool").transform;
            _networkService = networkService;
            
            _networkService.StopClientEvent += OnStopClient;
        }

        private GameObject SpawnHandler(SpawnMessage msg)
        {
            NetworkClient.GetPrefab(msg.assetId, out var prefab);
            return Get(prefab, msg.position, msg.rotation);
        }

        private void UnSpawnHandler(GameObject spawned) => Return(spawned);

        public void CreateNew(int count, params GameObject[] prefabs)
        {
            foreach (var prefab in prefabs)
            {
                RegisterPrefabs(prefab);
                _poolObjectCounts[prefab.name] = new PoolObjectCount();

                for (var i = 0; i < count; i++)
                    CreateNew(prefab);
            }
        }
        
        public void RegisterPrefabs(params GameObject[] prefabs)
        {
            foreach (var prefab in prefabs)
                NetworkClient.RegisterPrefab(prefab, SpawnHandler, UnSpawnHandler);
        }

        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject gameObject = null;
            var name = $"{prefab.name}(Clone)";

            if (!_cached.ContainsKey(name) || _cached[name].Count == 0)
                CreateNew(prefab);

            gameObject = _cached[name].Pop();
            SetCount(prefab, 1, -1);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            return gameObject;
        }

        public void Return(GameObject gameObject)
        {
            if (!_cached.ContainsKey(gameObject.name))
                _cached[gameObject.name] = new Stack<GameObject>();

            gameObject.SetActive(false);
            _cached[gameObject.name].Push(gameObject);
            SetCount(gameObject, -1, 1);
        }
        
        public bool IsCreated(GameObject prefab) => _cached.ContainsKey($"{prefab.name}(Clone)");

        private void CreateNew(GameObject prefab)
        {
            var gameObject = Engine.Instantiate(prefab, Vector3.zero, Quaternion.identity, _container);
            gameObject.SetActive(false);

            if (!_cached.ContainsKey(gameObject.name))
                _cached[gameObject.name] = new Stack<GameObject>();

            _cached[gameObject.name].Push(gameObject);
            SetCount(gameObject, 0, 1);
        }

        private void SetCount(GameObject prefab, int active, int cached)
        {
            var name = prefab.name.Replace("(Clone)", "");
            
            if (!_poolObjectCounts.ContainsKey(name))
                return;
            
            _poolObjectCounts[name].Active += active;
            _poolObjectCounts[name].Cached += cached;
            
            OnCountChanged?.Invoke(name, _poolObjectCounts[name]);
        }

        private void OnStopClient()
        {
            foreach (var poolObjectCount in _poolObjectCounts)
            {
                poolObjectCount.Value.Active = 0;
                poolObjectCount.Value.Cached = 0;
                
                OnCountChanged?.Invoke(poolObjectCount.Key, poolObjectCount.Value);
            }
        }

        public void Initialize()
        {
        }

        public void Destroy()
        {
            _networkService.StopClientEvent -= OnStopClient;
        }

        public class PoolObjectCount
        {
            public int Active;
            public int Cached;
        }
    }
}