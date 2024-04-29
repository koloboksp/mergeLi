using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIGameScreen_Coins : MonoBehaviour, ICoinsReceiver
    {
        public event Action OnClick;

        [SerializeField] private RectTransform _root;
        [SerializeField] private Transform _anchor;
        [SerializeField] private Button _clickableArea;
        [SerializeField] private Text _amountLabel;

        [SerializeField] private AnimationCurve _upScaleEffect;

        private int _amount = 0;
        private CancellationTokenSource _cancellationTokenSource;

        public Transform Anchor => _anchor;
        public bool IsActive => gameObject.activeInHierarchy;
       
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

        public void Add(int additionalAmount, bool force)
        {
            _amount += additionalAmount;
            
            _amountLabel.text = _amount.ToString();

            if (!force)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _cancellationTokenSource = new CancellationTokenSource();
                _ = UpScaleAsync(_cancellationTokenSource.Token, Application.exitCancellationToken);
            }
        }

        private async Task UpScaleAsync(CancellationToken cancellationToken, CancellationToken exitToken)
        {
            var time = 0.5f;
            var timer = 0.0f;
            
            while (timer < time)
            {
                exitToken.ThrowIfCancellationRequested();
                
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

        public void MakeSingle()
        {
            gameObject.SetActive(true);

            var others = FindObjectsOfType<UIGameScreen_Coins>();
            foreach (var other in others)
            {
                if (other != this)
                    other.gameObject.SetActive(false);
            }
        }
        
        public void Receive(int amount)
        {
            Add(amount, false);
        }
    }
}