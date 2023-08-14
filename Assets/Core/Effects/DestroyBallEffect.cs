using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Core.Effects
{
    public class DestroyBallEffect : MonoBehaviour
    {
        [SerializeField] List<DestroyBallEffectColorVariant> _colorVariants;
        [SerializeField] private float _duration = 2.0f;
        
        public void Run(Ball ball)
        {
            var colorIndex = ball.GetColorIndex(_colorVariants.Count);
            var variantInstance = GameObject.Instantiate(_colorVariants[colorIndex], transform);
            variantInstance.Run();
            
            Destroy(this.gameObject, _duration);
        }
    }
}