using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Core.Effects
{
    public class DestroyBallEffect : MonoBehaviour
    {
        [SerializeField] List<DestroyBallEffectColorVariant> _colorVariants;
        [SerializeField] private float _duration = 2.0f;
        [SerializeField] private float _delayScaler = 0.15f;

        public void Run(Ball ball, float delay)
        {
            var colorIndex = ball.GetColorIndex(_colorVariants.Count);
            
            StartCoroutine(StartEffect(_colorVariants[colorIndex], delay * _delayScaler));
        }

        IEnumerator StartEffect(DestroyBallEffectColorVariant variant, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            var variantInstance = Instantiate(variant, transform);
            variantInstance.Run();
            
            yield return new WaitForSeconds(_duration);
            Destroy(this.gameObject);
        }
    }
}