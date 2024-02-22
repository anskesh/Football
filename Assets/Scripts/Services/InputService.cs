using Configurations;
using Football.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Services
{
    public class InputService : IService<InputConfiguration>
    {
        public InputConfiguration Configuration { get; set; }
        public Camera Camera { get; private set; }

        public InputService(InputConfiguration inputConfiguration)
        {
            Configuration = inputConfiguration;
            
            Engine.CreateObject("Input", null, typeof(EventSystem), typeof(StandaloneInputModule));
            Camera = Engine.CreateObject("Camera", null, typeof(Camera)).GetComponent<Camera>();
        }

        public void SetParent(Transform parent)
        {
            Camera.transform.SetParent(parent, false);
        }

        public void ResetCamera() => SetParent(Engine.Behaviour.transform);
        
        public bool IsPressedFireButton() => Input.GetButton("Fire1");

        public bool IsFireButtonUp() => Input.GetButtonUp("Fire1");
        
        public Vector3 GetMousePosition()
        {
            var mouse = Input.mousePosition;
            var ray = Camera.ScreenPointToRay(mouse);
            
            var position = Vector3.zero;

            if (Physics.Raycast(ray, out var hit, 100, Configuration.LayerMask))
                position = hit.point;
            
            return position;
        }
        
        public void Initialize()
        {
        }

        public void Destroy()
        {
        }
    }
}
