using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class UIUpScaleEffect : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private AnimationCurve _upScaleEffect;

        public RectTransform Root => _root;
        
        private CancellationTokenSource _cancellationTokenSource;

        private void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }
        
        public void Add()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _ = UpScaleAsync(_cancellationTokenSource.Token);
        }
        
        private async Task UpScaleAsync(CancellationToken cancellationToken)
        {
            var time = 0.5f;
            var timer = 0.0f;
            while (timer < time)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                
                var nTimer = timer / time;
                _root.localScale = Vector3.one * _upScaleEffect.Evaluate(nTimer);
                await Task.Yield();

                timer += Time.deltaTime;
            }
        }
    }
}