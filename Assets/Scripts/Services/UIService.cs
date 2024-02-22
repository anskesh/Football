using System;
using System.Collections.Generic;
using Configurations;
using Football.Core;
using UI;
using UnityEngine;

namespace Services
{
    public class UIService : IService<UIConfiguration>
    {
        public UIConfiguration Configuration { get; set; }

        private List<View> _views = new List<View>();
        private Dictionary<Type, View> _cached = new Dictionary<Type, View>();
        private Transform _container;

        public UIService(UIConfiguration uiConfiguration)
        {
            _container = Engine.CreateObject("UIContainer").transform;
            Configuration = uiConfiguration;

            foreach (var view in Configuration.Views)
            {
                var ui = Engine.Instantiate(view, Vector3.zero, Quaternion.identity, _container);
                AddUI(ui);
            }
        }

        public T GetUI<T>() where T : View
        {
            if (_cached.ContainsKey(typeof(T)))
                return (T) _cached[typeof(T)];
            
            foreach (var view in _views)
            {
                if (view is not T) 
                    continue;
                
                _cached[typeof(T)] = view;
                return (T) view;
            }

            throw new Exception("UI doesn't exists.");
        }

        private void AddUI(View view)
        {
            if (_views.Contains(view))
                return;
            
            _views.Add(view);
        }
        
        public void Initialize()
        {
        }

        public void Destroy()
        {
        }
    }
}