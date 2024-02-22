using System;
using Configurations;
using Football;
using Football.Core;
using Mirror;
using UI;
using UnityEngine;
using NetworkManager = Football.NetworkManager;

namespace Services
{
    public class NetworkService : IService<NetworkConfiguration>
    {
        public Action ClientConnectedEvent;
        public Action ClientDisconnectedEvent;
        public Action StopClientEvent;
        
        public NetworkConfiguration Configuration { get; set; }
        public ScoreManager ScoreManager { get; private set; }
        public EColor ColorType { get; set; }
        public FootballField FootballField { get; private set; }

        private NetworkManager _networkManager;
        private UIService _uiService;
        
        public NetworkService(NetworkConfiguration networkConfiguration, UIService uiService)
        {
            Configuration = networkConfiguration;
            _uiService = uiService;
        }

        public void SetNetworkManager(NetworkManager manager)
        {
            _networkManager = manager;
            _networkManager.OnServerConnectedEvent += OnServerConnected;
            _networkManager.OnClientConnectedEvent += OnClientConnected;
            _networkManager.OnClientDisconnectedEvent += OnClientDisconnected;
            _networkManager.OnStopClientEvent += OnStopClient;
            
            _uiService.GetUI<InGameUI>().Render(_networkManager.maxConnections);
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

        private void OnClientConnected()
        {
            ClientConnectedEvent?.Invoke();
            _uiService.GetUI<InGameUI>().UpdateColorSlider(ColorType);
        }
        
        private void OnServerConnected()
        {
            if (ScoreManager)
                return;
            
            ScoreManager = Engine.Instantiate(Configuration.ScoreManager, Vector3.zero, Quaternion.identity).GetComponent<ScoreManager>();
            NetworkServer.Spawn(ScoreManager.gameObject);
        }

        private void OnClientDisconnected()
        {
            ClientDisconnectedEvent?.Invoke();
        }

        private void OnStopClient()
        {
            StopClientEvent?.Invoke();
        }

        public void Initialize()
        {
        }

        public void Destroy()
        {
            _networkManager.OnServerConnectedEvent -= OnServerConnected;
        }
    }
}