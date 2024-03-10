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
        
        public void Run(int colorIndex, float delay)
        {
            var wrapColorIndex = colorIndex % _colorVariants.Count;
            _ = StartEffectAsync(_colorVariants[wrapColorIndex], delay * _delayScaler, Application.exitCancellationToken);
        }

        private async Task StartEffectAsync(DestroyBallEffectColorVariant variant, float delay, CancellationToken cancellationToken)
        {
            try
            {
                await AsyncExtensions.WaitForSecondsAsync(delay, cancellationToken);

               // var variantInstance = Instantiate(variant, transform);
               // variantInstance.Run();

                await AsyncExtensions.WaitForSecondsAsync(_duration, cancellationToken);

                Destroy(gameObject);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}