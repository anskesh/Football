using System.Linq;
using Football.Core;
using Mirror;
using Services;
using UI;
using NetworkBehaviour = Mirror.NetworkBehaviour;

namespace Football
{
    public class ScoreManager : NetworkBehaviour
    {
        private readonly SyncList<int> _scores = new SyncList<int>();
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
                    _scores.Add(0);
            }
            
            Engine.GetService<NetworkService>().SetScoreManager(this);
            _ui.UpdateScore(_scores.ToList());
        }

        [Server]
        public void ScoreGoal(NetworkIdentity sender, int receiver)
        {
            var gate = sender.GetComponent<Gate>();

            if (gate)
            {
                if (gate.ID == receiver)
                    return;
                
                IncreaseScore(gate.ID);
            }
            
            DecreaseScore(receiver);
        }
        
        [Server]
        private void IncreaseScore(int id)
        {
            if (_scores.Count <= id)
                return;

            _scores[id]++;
            OnScoreChanged(id, _scores[id]);
        }
        
        [Server]
        private void DecreaseScore(int id)
        {
            if (_scores.Count <= id)
                return;

            _scores[id]--;
            OnScoreChanged(id, _scores[id]);
        }

        [ClientRpc]
        private void OnScoreChanged(int id, int score)
        {
            _ui.UpdateScore(id, score);
        }

        [Server]
        public void ResetScore(int id)
        {
            if (_scores.Count <= id)
                return;
            
            _scores[id] = 0;
            OnScoreChanged(id, _scores[id]);
        }
    }
}