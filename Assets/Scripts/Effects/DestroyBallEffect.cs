using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Utils;
using UnityEngine;

namespace Core.Effects
{
    public class DestroyBallEffect : MonoBehaviour
    {
        [SerializeField] private DestroyBallEffectColorVariant _defaultColorVariant;
        [SerializeField] private float _duration = 2.0f;
        [SerializeField] private AudioClip[] _clips;

        private DependencyHolder<SoundsPlayer> _soundsPlayer;
        
        public void Run(Color mainColor)
        {
            _ = StartEffectAsync(mainColor, Application.exitCancellationToken);
        }

        private async Task StartEffectAsync(Color mainColor, CancellationToken cancellationToken)
        {
            try
            {
                var variantInstance = Instantiate(_defaultColorVariant, transform);
                variantInstance.Run(mainColor);
                _soundsPlayer.Value.Play(_clips[UnityEngine.Random.Range(0, _clips.Length)]);
                
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