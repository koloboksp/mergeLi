using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UICoins : MonoBehaviour
    {
        public event Action OnClick;
        
        [SerializeField] private Button _clickableArea;
        [SerializeField] private Text _amountLabel;

        public void Awake()
        {
            _clickableArea.onClick.AddListener(ClickableArea_OnClick);
        }

        private void ClickableArea_OnClick()
        {
            OnClick?.Invoke();
        }

        public void SetCoins(int score)
        {
            _amountLabel.text = score.ToString();
        }
    }
}