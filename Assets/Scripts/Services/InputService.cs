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

        public InputService()
        {
            Configuration = Engine.GetConfiguration<InputConfiguration>();
            
            Engine.CreateObject("Input", null, typeof(EventSystem), typeof(StandaloneInputModule));
            Camera = Engine.CreateObject("Camera", null, typeof(Camera)).GetComponent<Camera>();
        }

        public void ResetCamera()
        {
            Camera.transform.SetParent(Engine.Behaviour.transform, false);
        }
        
        public bool IsPressedFireButton()
        {                                                       
            return Input.GetButton("Fire1");
        }

        public bool IsFireButtonUp()
        {
            return Input.GetButtonUp("Fire1");
        }
        
        public Vector3 GetMousePosition()
        {
            var mouse = Input.mousePosition;
            var ray = Camera.ScreenPointToRay(mouse);
            
            var position = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit, 100, Configuration.LayerMask))
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
