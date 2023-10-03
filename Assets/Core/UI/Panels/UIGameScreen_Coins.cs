using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIGameScreen_Coins : MonoBehaviour
    {
        public event Action OnClick;

        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _clickableArea;
        [SerializeField] private Text _amountLabel;

        [SerializeField] private AnimationCurve _upScaleEffect;

        private int _amount = 0;
        private CancellationTokenSource _cancellationTokenSource;
        
        private void Awake()
        {
            _clickableArea.onClick.AddListener(ClickableArea_OnClick);
        }

        private void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }
        
        private void ClickableArea_OnClick()
        {
            OnClick?.Invoke();
        }

        public void Add(int additionalAmount)
        {
            _amount += additionalAmount;
            
            _amountLabel.text = _amount.ToString();

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            UpScale(_cancellationTokenSource.Token);
        }

        private async Task UpScale(CancellationToken cancellationToken)
        {
            var time = 0.5f;
            var timer = 0.0f;
            while (timer < time)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                
                var nTimer = timer / time;
                _root.localScale = Vector3.one * _upScaleEffect.Evaluate(nTimer);
                await Task.Yield();

                timer += Time.deltaTime;
            }
        }

        public void Set(int amount)
        {
            _amount = amount;
            _amountLabel.text = _amount.ToString();
        }
    }
}