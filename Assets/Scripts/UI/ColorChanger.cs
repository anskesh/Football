using System;
using Configurations;
using Football.Core;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Football
{
    public class ColorChanger : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Transform _container;
        [SerializeField] private Button _template;

        private void Awake()
        {
            var colors = Engine.GetConfiguration<CommonConfiguration>().Colors;

            foreach (var color in colors)
            {
                if (color.Type == EColor.Default)
                    continue;
                
                var button = Instantiate(_template, _container);
                button.GetComponent<Image>().color = color.Value;
                button.onClick.AddListener(() => ChangeColor(color.Type));
            }
        }

        private void Start()
        {
            ChangeColor(EColor.Red);
        }

        private void ChangeColor(EColor color)
        {
            Engine.GetService<NetworkService>().ColorType = color;
            
            var colorName = "";

            switch (color)
            {
                case EColor.Red:
                    colorName = "Красный";
                    break;
                case EColor.Green:
                    colorName = "Зеленый";
                    break;
                case EColor.Blue:
                    colorName = "Синий";
                    break;
                case EColor.Yellow:
                    colorName = "Желтый";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _text.text = colorName;
        }
    }
}