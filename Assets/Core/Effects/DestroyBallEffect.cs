using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;
using UnityEngine;

namespace Core.Effects
{
    public class DestroyBallEffect : MonoBehaviour
    {
        [SerializeField] List<DestroyBallEffectColorVariant> _colorVariants;
        [SerializeField] private float _duration = 2.0f;
        [SerializeField] private float _delayScaler = 0.15f;

        private CancellationTokenSource _cancellationTokenSource;

        private void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public void Run(int colorIndex, float delay)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            var wrapColorIndex = colorIndex % _colorVariants.Count;
            _ = StartEffectAsync(_colorVariants[wrapColorIndex], delay * _delayScaler, _cancellationTokenSource.Token);
        }

        private async Task StartEffectAsync(DestroyBallEffectColorVariant variant, float delay, CancellationToken cancellationToken)
        {
            await ApplicationController.WaitForSecondsAsync(delay, cancellationToken);
            
            var variantInstance = Instantiate(variant, transform);
            variantInstance.Run();
            
            await ApplicationController.WaitForSecondsAsync(_duration, cancellationToken);
            
            Destroy(gameObject);
        }
    }
}