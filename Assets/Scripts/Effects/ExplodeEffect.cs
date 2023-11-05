using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Effects
{
    public class ExplodeEffect : MonoBehaviour
    {
        [SerializeField] private float _duration = 2.0f;
        [SerializeField] private float _delayScaler = 0.15f;
        [SerializeField] private GameObject _effectRoot; 
        private CancellationTokenSource _cancellationTokenSource;
        
        public float Duration => _duration;

        private void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public void Run(float delay)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
           
            _ = StartEffectAsync(delay * _delayScaler, _cancellationTokenSource.Token);
        }

        private async Task StartEffectAsync(float delay, CancellationToken cancellationToken)
        {
            await AsyncExtensions.WaitForSecondsAsync(delay, cancellationToken);
            _effectRoot.SetActive(true);
            await AsyncExtensions.WaitForSecondsAsync(_duration, cancellationToken);
            
            Destroy(gameObject);
        }
    }
}