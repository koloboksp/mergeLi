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
        
        public float Duration => _duration;
        
        public void Run(float delay)
        {
            _ = StartEffectAsync(delay * _delayScaler, Application.exitCancellationToken);
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