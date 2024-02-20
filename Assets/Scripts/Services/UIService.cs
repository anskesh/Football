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
        private Transform _container;

        public UIService()
        {
            _container = Engine.CreateObject("UIContainer").transform;
            Configuration = Engine.GetConfiguration<UIConfiguration>();

            foreach (var view in Configuration.Views)
            {
                var ui = Engine.Instantiate(view, Vector3.zero, Quaternion.identity, _container);
                AddUI(ui);
            }
        }

        public T GetUI<T>() where T : View
        {
            foreach (var view in _views)
            {
                if (view is T)
                    return (T) view;
            }

            throw new Exception("UI doesn't.");
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