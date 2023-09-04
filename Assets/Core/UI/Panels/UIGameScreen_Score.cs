using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIGameScreen_Score : MonoBehaviour
    {
        [SerializeField] private Text _sessionScoreLabel;
        [SerializeField] private Text _bestScoreLabel;
        [SerializeField] private Text _scoreLabel;
        [SerializeField] private Text _nextGoalScoreLabel;
        [SerializeField] private UIProgressBar _partProgressBar;

        public void SetSessionScore(int sessionScore, int bestSessionScore)
        {
            _sessionScoreLabel.text = sessionScore.ToString();
            _bestScoreLabel.text = bestSessionScore.ToString();
        }
        
        public void SetNextGoalScore(int points, int cost)
        {
            _scoreLabel.text = points.ToString();
            _nextGoalScoreLabel.text = cost.ToString();

            _partProgressBar.SetProgress((float)points / (float)cost);
        }

        public void SetScoreMarks(IEnumerable<int> marks)
        {
            _partProgressBar.SetMarks(marks);
        }
    }
}