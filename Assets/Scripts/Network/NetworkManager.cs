using System;
using Football.Core;
using Mirror;
using Services;
using UnityEngine;

namespace Football
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public Action OnServerConnectedEvent;
        public Action OnClientConnectedEvent;
        public Action OnClientDisconnectedEvent;
        public Action OnStopClientEvent;

        public override void Start()
        {
            base.Start();

            Engine.GetService<NetworkService>().SetNetworkManager(this);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            
            Debug.Log("Server connect");
            NetworkServer.RegisterHandler<PlayerSettings>(CreateCharacter);
            OnServerConnectedEvent?.Invoke();
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            
            Debug.Log("Client connect");
            var color = Engine.GetService<NetworkService>().ColorType;
            
            var message = new PlayerSettings()
            {
                Color = color
            };
                
            NetworkClient.Send(message);
            OnClientConnectedEvent?.Invoke();
        }

        private void CreateCharacter(NetworkConnectionToClient conn, PlayerSettings message)
        {
            var gate = Engine.GetService<NetworkService>().FootballField.GetField();
            NetworkServer.AddPlayerForConnection(conn, gate.gameObject);
            gate.RPCConnectPlayer(message.Color);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log("Disconnect server");
            
            if (conn.identity.TryGetComponent<Gate>(out var gate))
                gate.ResetPlayer();
            
            base.OnServerDisconnect(conn);
            NetworkServer.Spawn(gate.gameObject);
        }

        public override void OnClientDisconnect()
        {
            Debug.Log("Disconnect client");
            Engine.GetService<InputService>().ResetCamera();
            
            base.OnClientDisconnect();
            
            OnClientDisconnectedEvent?.Invoke();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            
            Debug.Log("StopClient");
            OnStopClientEvent?.Invoke();
        }
    }
}