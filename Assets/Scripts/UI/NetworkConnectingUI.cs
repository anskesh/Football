using Football.Core;
using Services;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Football.UI
{
    public class NetworkConnectingUI : View
    {
        [SerializeField] private Button _hostBtn;
        [SerializeField] private Button _clientBtn;

        private NetworkService _networkService;

        protected override void Awake()
        {
            base.Awake();
            
            _hostBtn.onClick.AddListener(OnHostBtnClicked);
            _clientBtn.onClick.AddListener(OnClientBtnClicked);
        }

        private void Start()
        {
            _networkService = Engine.GetService<NetworkService>();
            
            _networkService.ClientDisconnectedEvent += Show;
            _networkService.ClientConnectedEvent += Hide;
        }

        private void OnDestroy()
        {
            _hostBtn.onClick.RemoveListener(OnHostBtnClicked);
            _clientBtn.onClick.RemoveListener(OnClientBtnClicked);
            
            _networkService.ClientDisconnectedEvent -= Show;
            _networkService.ClientConnectedEvent -= Hide;
        }

        private void OnHostBtnClicked()
        {
            _networkService.StartHost();
        }

        private void OnClientBtnClicked()
        {
            _networkService.StartClient();
        }
    }
}