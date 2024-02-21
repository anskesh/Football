﻿using System;
using Football.Core;
using Football.UI;
using Mirror;
using Services;
using UnityEngine;

namespace Football
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public Action OnServerConnected;
        public Action OnClientConnected;
        public Action OnClientDisconnected;

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
            OnServerConnected?.Invoke();
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            
            Debug.Log("client connect");
            var color = Engine.GetService<NetworkService>().Color;
            
            var message = new PlayerSettings()
            {
                Color = color
            };
                
            NetworkClient.Send(message);
            OnClientConnected?.Invoke();
            Engine.GetService<UIService>().GetUI<NetworkConnectingUI>().Hide();
        }

        private void CreateCharacter(NetworkConnectionToClient conn, PlayerSettings message)
        {
            Debug.Log("Create character");
            var gate = Engine.GetService<NetworkService>().FootballField.GetField();
            NetworkServer.AddPlayerForConnection(conn, gate.gameObject);
            gate.RPCConnectPlayer(conn, message.Color);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log("Disconnect");
            conn.identity.GetComponent<Gate>().ResetPlayer();
            base.OnServerDisconnect(conn);
        }

        public override void OnClientDisconnect()
        {
            Engine.GetService<InputService>().ResetCamera();
            base.OnClientDisconnect();
            
            Engine.GetService<UIService>().GetUI<NetworkConnectingUI>().Show();
            OnClientDisconnected?.Invoke();
        }
    }
}