using System.Collections.Generic;
using System.Linq;
using Configurations;
using Football;
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
        [SerializeField] private Slider _slider;
        
        private List<PlayerScore> _scores = new List<PlayerScore>();
        private List<CommonConfiguration.ColorSettings> _colors;

        private NetworkService _networkService;
        private Image _sliderImage;

        protected override void Awake()
        {
            base.Awake();

            _quitButton.onClick.AddListener(OnQuitButtonClicked);
            _colors = Engine.GetConfiguration<CommonConfiguration>().Colors;
            _sliderImage = _slider.fillRect.GetComponent<Image>();
        }

        private void Start()
        {
            _networkService = Engine.GetService<NetworkService>();
            
            _networkService.ClientConnectedEvent += Show;
            _networkService.ClientDisconnectedEvent += Hide;
        }

        private void OnDestroy()
        {
            _networkService.ClientConnectedEvent -= Show;
            _networkService.ClientDisconnectedEvent -= Hide;
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

        #region Score

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

        #endregion

        public void UpdateTime(float value)
        {
            _slider.value = value;
        }

        public void UpdateColorSlider(EColor color)
        {
            _sliderImage.color = _colors.First(x => x.Type == color).Value;
        }
        
        private void OnQuitButtonClicked()
        {
            if (NetworkServer.activeHost)
                _networkService.StopHost();
            else
                _networkService.StopClient();
        }
    }
}