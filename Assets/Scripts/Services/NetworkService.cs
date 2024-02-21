using System;
using Configurations;
using Football;
using Football.Core;
using Mirror;
using UI;
using UnityEngine;
using NetworkBehaviour = Football.Core.NetworkBehaviour;
using NetworkManager = Football.NetworkManager;

namespace Services
{
    public class NetworkService : IService<NetworkConfiguration>
    {
        public Action OnClientConnected;
        public Action OnClientDisconnected;
        
        public NetworkConfiguration Configuration { get; set; }
        public NetworkBehaviour Behaviour { get; private set; }
        
        public ScoreManager ScoreManager { get; private set; }
        public EColor Color { get; set; }
        public FootballField FootballField { get; private set; }

        private NetworkManager _networkManager;
        
        public NetworkService()
        {
            Configuration = Engine.GetConfiguration<NetworkConfiguration>();
            Behaviour = Engine.CreateObject("NetworkBehaviour", null, typeof(NetworkBehaviour)).GetComponent<NetworkBehaviour>();

            UpdateNetworkManager();
        }

        public void SetNetworkManager(NetworkManager manager)
        {
            _networkManager = manager;
            _networkManager.OnServerConnected += CreateScoreManager;
            _networkManager.OnClientConnected += () =>
            {
                OnClientConnected?.Invoke();
            };
            
            _networkManager.OnClientDisconnected += () =>
            {
                OnClientDisconnected?.Invoke();
            };
            
            Engine.GetService<UIService>().GetUI<InGameUI>().Render(_networkManager.maxConnections);
        }

        private void UpdateNetworkManager()
        {
            _networkManager = Engine.Instantiate(Configuration.Manager, Vector3.zero, Quaternion.identity);
        }
        
        public void SetField(FootballField field)
        {
            FootballField = field;
        }
        
        public void SetScoreManager(ScoreManager manager)
        {
            ScoreManager ??= manager;
        }

        public void StartHost() => _networkManager.StartHost();
        public void StartClient() => _networkManager.StartClient();
        
        public void StopHost() => _networkManager.StopHost();
        public void StopClient() => _networkManager.StopClient();
        

        private void CreateScoreManager()
        {
            if (ScoreManager)
                return;
            
            ScoreManager = Engine.Instantiate(Configuration.ScoreManager, Vector3.zero, Quaternion.identity).GetComponent<ScoreManager>();
            NetworkServer.Spawn(ScoreManager.gameObject);
        }

        public void Initialize()
        {
        }

        public void Destroy()
        {
            _networkManager.OnServerConnected -= CreateScoreManager;
        }
    }
}