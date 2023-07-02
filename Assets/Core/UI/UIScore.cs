using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIScore : MonoBehaviour
    {
        [SerializeField] private Text _bestScoreLabel;
        [SerializeField] private Text _scoreLabel;
        [SerializeField] private Image _bar;

        public void SetScore(int score)
        {
            _scoreLabel.text = score.ToString();
        }
    }
}