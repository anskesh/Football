using Configurations;
using Football;
using Football.Core;
using Mirror;
using UnityEngine;
using NetworkBehaviour = Football.Core.NetworkBehaviour;
using NetworkManager = Football.NetworkManager;

namespace Services
{
    public class NetworkService : IService<NetworkConfiguration>
    {
        public NetworkConfiguration Configuration { get; set; }
        public NetworkBehaviour Behaviour { get; private set; }
        
        public ScoreManager ScoreManager { get; private set; }
        public NetworkManager NetworkManager { get; private set; }

        public EColor Color { get; set; }
        public FootballField FootballField { get; private set; }

        public NetworkService()
        {
            Configuration = Engine.GetConfiguration<NetworkConfiguration>();
            Behaviour = Engine.CreateObject("NetworkBehaviour", null, typeof(NetworkBehaviour)).GetComponent<NetworkBehaviour>();

            NetworkManager = Engine.Instantiate(Configuration.Manager, Vector3.zero, Quaternion.identity);
            NetworkManager.OnServerConnected += CreateScoreManager;
        }
        
        public void SetField(FootballField field)
        {
            FootballField = field;
        }
        
        public void SetScoreManager(ScoreManager manager)
        {
            ScoreManager ??= manager;
        }

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
            NetworkManager.OnServerConnected -= CreateScoreManager;
        }
    }
}