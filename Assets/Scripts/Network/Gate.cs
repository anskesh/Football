using System;
using System.Linq;
using Configurations;
using Football.Core;
using Mirror;
using Services;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using NetworkBehaviour = Mirror.NetworkBehaviour;
using Random = UnityEngine.Random;

namespace Football
{
    public class Gate : NetworkBehaviour
    {
        [SyncVar] protected bool _isAvailable = true;

        [SyncVar(hook = nameof(OnColorChange))]
        private EColor _color;

        public int ID { get; private set; }
        public bool IsAvailable => _isAvailable;

        [SerializeField] private MeshRenderer[] _meshRenderers;
        [SerializeField] private Transform _cameraPosition;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Cannon _cannon;

        private CommonConfiguration _commonConfiguration;
        private NetworkService _networkService;

        private Vector3 _target;
        private Vector3 _offset = Vector3.zero;
        private Vector3 _defaultPosition;
        private float _offsetValue;

        private bool _isMoving = false;
        private bool _isX = true;

        protected override void OnValidate()
        {
            base.OnValidate();

            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _cannon = GetComponentInChildren<Cannon>();
        }

        private void Awake()
        {
            _defaultPosition = transform.position;
            _commonConfiguration = Engine.GetConfiguration<CommonConfiguration>();
            _networkService = Engine.GetService<NetworkService>();

            if (Math.Round(transform.eulerAngles.y) == 0 || Math.Round(transform.eulerAngles.y) == 180)
                _isX = false;
        }

        public void Start()
        {
            UpdateColor();
        }

        private void Update()
        {
            if (_isMoving == false)
                return;

            transform.position = Vector3.MoveTowards(transform.position, _target, 10 * Time.deltaTime);

            if (!(Vector3.Distance(transform.position, _target) <= 0.5f))
                return;

            if (_isX)
                _target.x *= -1;
            else
                _target.z *= -1;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isAvailable || !isLocalPlayer)
                return;

            if (other.TryGetComponent(out Ball ball))
            {
                if (ball.Owner && ball.Owner.clientStarted)
                    ScoreGoal(ball.Owner, ID);
            }
        }

        [Command]
        private void ScoreGoal(NetworkIdentity owner, int id)
        {
            _networkService.ScoreManager.ScoreGoal(owner, id);
        }

        public void Init(float offset, int id)
        {
            _offsetValue = offset;
            ID = id;
            _text.text = (ID + 1).ToString();
            gameObject.SetActive(false);
        }

        public void ResetPlayer()
        {
            ChangeState(true);
            transform.position = _defaultPosition;
            _cannon.ResetRotation();
            Engine.GetService<NetworkService>().ScoreManager.ResetScore(ID);
            _color = EColor.Default;
        }

        public Bounds GetBounds()
        {
            return _meshRenderers[0].bounds;
        }

        private void StartMoving()
        {
            _offset = _isX ? new Vector3(_offsetValue, 0, 0) : new Vector3(0, 0, _offsetValue);
            _target = _defaultPosition - _offset;

            var rand = Random.Range(0, 2);

            if (rand == 0)
                if (_isX)
                    _target.x *= -1;
                else
                    _target.z *= -1;

            _isMoving = true;
        }

        private void UpdateColor()
        {
            var colorValue = _commonConfiguration.Colors.First(x => x.Type == _color).Value;
            UpdateColor(colorValue);
        }

        private void UpdateColor(Color color)
        {
            foreach (var meshRenderer in _meshRenderers)
                meshRenderer.material.color = color;
            
            Engine.GetService<UIService>().GetUI<InGameUI>().UpdateColor(ID, color);
        }

        private void ChangeState(bool state)
        {
            _isAvailable = state;
        }

        [TargetRpc]
        public void RPCConnectPlayer(NetworkConnectionToClient conn, EColor color)
        {
            ChangeColor(color);
            gameObject.SetActive(true);
            var camera = Engine.GetService<InputService>().Camera;
            camera.transform.SetParent(_cameraPosition, false);
            ChangeState(false);

            StartMoving();
        }

        [Command]
        private void ChangeColor(EColor color)
        {
            _color = color;
        }

        private void OnColorChange(EColor oldColor, EColor color)
        {
            _color = color;
            UpdateColor();

            Debug.Log($"Changed color to {color}");
        }
    }
}