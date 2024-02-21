using System;
using Football.Core;
using Football.UI;
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
            Engine.GetService<UIService>().GetUI<NetworkConnectingUI>().Hide();
        }

        private void CreateCharacter(NetworkConnectionToClient conn, PlayerSettings message)
        {
            var gate = Engine.GetService<NetworkService>().FootballField.GetField();
            NetworkServer.AddPlayerForConnection(conn, gate.gameObject);
            gate.RPCConnectPlayer(conn, message.Color);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log("Disconnect server");
            conn.identity.GetComponent<Gate>().ResetPlayer();
            
            base.OnServerDisconnect(conn);
        }

        public override void OnClientDisconnect()
        {
            Debug.Log("Disconnect client");
            Engine.GetService<InputService>().ResetCamera();
            
            base.OnClientDisconnect();
            
            Engine.GetService<UIService>().GetUI<NetworkConnectingUI>().Show();
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