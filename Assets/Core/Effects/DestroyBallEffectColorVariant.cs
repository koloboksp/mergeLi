using UnityEngine;

namespace Core.Effects
{
    public class DestroyBallEffectColorVariant : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _mainParticleSystem;
    
        public void Run()
        {
            _mainParticleSystem.Play(true);
        }
    }
}