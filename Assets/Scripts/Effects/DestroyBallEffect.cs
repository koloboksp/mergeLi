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
        [SerializeField] private AudioClip _clip;

        private DependencyHolder<SoundsPlayer> _soundsPlayer;
        
        public void Run(int colorIndex)
        {
            var wrapColorIndex = colorIndex % _colorVariants.Count;
            _ = StartEffectAsync(_colorVariants[wrapColorIndex], Application.exitCancellationToken);
        }

        private async Task StartEffectAsync(DestroyBallEffectColorVariant variant, CancellationToken cancellationToken)
        {
            try
            {
                var variantInstance = Instantiate(variant, transform);
                variantInstance.Run();
                _soundsPlayer.Value.Play(_clip);
                
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