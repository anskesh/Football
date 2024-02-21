using System.Collections.Generic;
using Football.Core;
using Mirror;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameUI : View
    {
        [SerializeField] private Button _quitButton;
        
        [SerializeField] private Transform _container;
        [SerializeField] private PlayerScore _scoreTemplate;
        
        private List<PlayerScore> _scores = new List<PlayerScore>();

        protected override void Awake()
        {
            base.Awake();

            _quitButton.onClick.AddListener(OnQuitButtonClicked);
            Engine.GetService<NetworkService>().OnClientConnected += Show;
            Engine.GetService<NetworkService>().OnClientDisconnected += Hide;
        }

        private void OnDestroy()
        {
            Engine.GetService<NetworkService>().OnClientConnected -= Show;
            Engine.GetService<NetworkService>().OnClientDisconnected -= Hide;
        }

        public void UpdateScore(List<int> scores)
        {
            for (var i = 0; i < scores.Count; i++)
                _scores[i].ChangeScore(scores[i]);
        }
        
        public void UpdateScore(int id, int score)
        {
            _scores[id].ChangeScore(score);
        }
        
        public void UpdateColor(int id, Color color)
        {
            _scores[id].ChangeColor(color);
        }
        
        public void Render(int count)
        {
            if (_scores.Count > 0)
                return;

            for (var i = 0; i < count; i++)
            {
                var score = Instantiate(_scoreTemplate, _container);
                score.InitID(i + 1);
                _scores.Add(score);
            }
        }
        
        private void OnQuitButtonClicked()
        {
            if (NetworkServer.activeHost)
                Engine.GetService<NetworkService>().StopHost();
            else
                Engine.GetService<NetworkService>().StopClient();
        }
    }
}