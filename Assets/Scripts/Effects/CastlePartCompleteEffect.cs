using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Effects
{
    public class CastlePartCompleteEffect : MonoBehaviour
    {
        [SerializeField] private float _duration = 2.0f;
        [SerializeField] private AudioClip _completeClip;
        [SerializeField] private GameObject _effectRoot;

        private DependencyHolder<SoundsPlayer> _soundPlayer;
        
        public float Duration => _duration;
        
        public void Run()
        {
            _ = StartEffectAsync(Application.exitCancellationToken);
        }

        private async Task StartEffectAsync(CancellationToken exitToken)
        {
            _soundPlayer.Value.Play(_completeClip);
            _effectRoot.SetActive(true);
            await AsyncExtensions.WaitForSecondsAsync(_duration, exitToken);
            
            Destroy(gameObject);
        }
    }
}