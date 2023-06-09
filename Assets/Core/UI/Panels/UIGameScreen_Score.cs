﻿using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIGameScreen_Score : MonoBehaviour
    {
        [SerializeField] private Text _bestScoreLabel;
        [SerializeField] private Text _scoreLabel;
        [SerializeField] private Text _nextGoalScoreLabel;
        [SerializeField] private Image _bar;

        public void SetScore(int score, int nextGoalScore)
        {
            _scoreLabel.text = score.ToString();
            _nextGoalScoreLabel.text = nextGoalScore.ToString();

            _bar.fillAmount = (float)score / (float)nextGoalScore;
        }
    }
}