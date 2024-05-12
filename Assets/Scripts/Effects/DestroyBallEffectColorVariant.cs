using UnityEngine;

namespace Core.Effects
{
    public class DestroyBallEffectColorVariant : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _mainParticleSystem;

        [SerializeField] private ParticleSystem _frontParticleSystem1;
        [SerializeField] private ParticleSystem _frontParticleSystem2;
        [SerializeField] private ParticleSystem _backParticleSystem1;
        [SerializeField] private ParticleSystem _backParticleSystem2;
        [SerializeField] private float _colorDarkening = 0.2f;
        
        public void Run(Color mainColor)
        {
            Color.RGBToHSV(mainColor, out var h, out var f, out var v);

            f *= 1.2f;
            
            var front1Color = Color.HSVToRGB(h, f * 1f, 1f);
            var front2Color = Color.HSVToRGB(h, f * 0.8f, 0.8f);
            var front1Module = _frontParticleSystem1.main;
            front1Module.startColor = front1Color;
            var front2Module = _frontParticleSystem2.main;
            front2Module.startColor = front2Color;
            
            var back1Color = Color.HSVToRGB(h, f * 0.4f, 0.4f);
            var back2Color = Color.HSVToRGB(h, f * 0.3f, 0.3f);
            var back1Module = _backParticleSystem1.main;
            back1Module.startColor = back1Color ;
            var back2Module = _backParticleSystem2.main;
            back2Module.startColor = back2Color;

            _mainParticleSystem.Play(true);
        }
    }
}