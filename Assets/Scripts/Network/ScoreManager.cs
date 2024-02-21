using System.Linq;
using Football.Core;
using Mirror;
using Services;
using UI;
using UnityEngine;
using NetworkBehaviour = Mirror.NetworkBehaviour;

namespace Football
{
    public class ScoreManager : NetworkBehaviour
    {
        public readonly SyncList<int> Scores = new SyncList<int>();

        private InGameUI _ui;

        private void Awake()
        {
            _ui = Engine.GetService<UIService>().GetUI<InGameUI>();
        }

        private void Start()
        {
            if (NetworkServer.activeHost)
            {
                for (var i = 0; i < NetworkServer.maxConnections; i++)
                    Scores.Add(0);
            }
            
            Engine.GetService<NetworkService>().SetScoreManager(this);
            _ui.UpdateScore(Scores.ToList());
        }

        [Server]
        public void ScoreGoal(NetworkIdentity sender, int receiver)
        {
            var gate = sender.GetComponent<Gate>();
            
            if (gate)
                IncreaseScore(gate.ID);
            
            DecreaseScore(receiver);
        }
        
        [Server]
        private void IncreaseScore(int id)
        {
            if (Scores.Count <= id)
                return;

            Scores[id]++;
            OnScoreChanged(id, Scores[id]);
        }
        
        [Server]
        private void DecreaseScore(int id)
        {
            if (Scores.Count <= id)
                return;

            Scores[id]--;
            OnScoreChanged(id, Scores[id]);
        }

        [ClientRpc]
        private void OnScoreChanged(int id, int score)
        {
            _ui.UpdateScore(id, score);
        }

        [Server]
        public void ResetScore(int id)
        {
            if (Scores.Count <= id)
                return;
            
            Scores[id] = 0;
            OnScoreChanged(id, Scores[id]);
        }
    }
}