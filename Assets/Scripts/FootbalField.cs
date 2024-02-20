using System.Collections.Generic;
using System.Linq;
using Football.Core;
using Services;
using UnityEngine;

namespace Football
{
    public class FootballField : MonoBehaviour
    {
        public IReadOnlyCollection<Gate> Gates => _gates;

        [SerializeField] private List<Gate> _gates = new List<Gate>();
        [SerializeField] private Transform _floor;

        private Vector3 _target;

        private void Awake()
        {
            var floorSize = _floor.GetComponent<MeshRenderer>().bounds.extents.x;
            var gateBounds = _gates[0].GetBounds();
            var gateSize = gateBounds.size.x * 2 + gateBounds.extents.z;

            var offset = floorSize - gateSize;

            for (var i = 0; i < _gates.Count; i++)
            {
                var gate = _gates[i];
                gate.Init(offset, i);
            }

            Engine.GetService<NetworkService>().SetField(this);
        }

        public Gate GetField()
        {
            return _gates.First(x => x.IsAvailable);
        }

        private void OnValidate()
        {
            _gates = GetComponentsInChildren<Gate>().ToList();
        }
    }
}