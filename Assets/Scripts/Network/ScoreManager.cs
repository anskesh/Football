using Football.Core;
using Mirror;
using Services;
using UnityEngine;
using NetworkBehaviour = Mirror.NetworkBehaviour;

namespace Football
{
    public class ScoreManager : NetworkBehaviour
    {
        public readonly SyncList<int> Scores = new SyncList<int>();

        private void Start()
        {
            if (!NetworkServer.activeHost)
            {
                Engine.GetService<NetworkService>().SetScoreManager(this);
                return;
            }
            
            for (var i = 0; i < NetworkServer.maxConnections; i++)
                Scores.Add(0);
        }

        public void CmdScoreGoal(NetworkIdentity sender, int receiver)
        {
            var gate = sender.GetComponent<Gate>();
            
            if (gate)
                CmdIncreaseScore(gate.Id);
            
            CmdDecreaseScore(receiver);
        }
        
        private void CmdIncreaseScore(int id)
        {
            if (Scores.Count <= id)
                return;

            Scores[id]++;
        }
        
        private void CmdDecreaseScore(int id)
        {
            if (Scores.Count <= id)
                return;

            Scores[id]--;
        }

        [Command]
        public void CmdResetScore(int id)
        {
            if (Scores.Count <= id)
                return;
            
            Scores[id] = 0;
        }
    }
}