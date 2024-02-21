using System;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class View : MonoBehaviour
    {
        [SerializeField] private bool _hideOnLoad;
        [SerializeField] private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            if (_hideOnLoad)
                Hide();
        }

        private void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}