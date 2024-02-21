using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerScore : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _id;
        [SerializeField] private TextMeshProUGUI _score;

        public void InitID(int id)
        {
            _id.text = id.ToString();
        }

        public void ChangeColor(Color color)
        {
            _image.color = color;
        }
        
        public void ChangeScore(int score)
        {
            _score.text = score.ToString();
        }
    }
}