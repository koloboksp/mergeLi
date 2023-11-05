using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Effects
{
    public class CollapsePointsEffect : MonoBehaviour
    {
        [SerializeField] private Text _points;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private AnimationCurve _movingSpeed;
        [SerializeField] private float _randomizeStartPosition = 0.1f;
        [SerializeField] private float _randomizeSideOffset = 0.1f;
        [SerializeField] private float _randomizeDelay = 0.1f;
        
        private CancellationTokenSource _cancellationTokenSource;

        public float Duration => _duration;
    
        private void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
        
        public void Run(int points, int coinsCount)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _points.text = points.ToString();

            var coinsEffectReceiver = GameObject.FindObjectOfType<CoinsEffectReceiver>();

            for (int i = 0; i < coinsCount; i++)
            {
                Vector3 randDirection = Random.insideUnitCircle;
                var startPosition = transform.position + randDirection * _randomizeStartPosition;
                var endPosition = coinsEffectReceiver.transform.position;
                var vecToReceiver = endPosition - startPosition;
                var distanceToReceiver = vecToReceiver.magnitude;
                var dirToReceiver = vecToReceiver.normalized;
                var midPoint = startPosition + vecToReceiver * 0.5f +
                               Vector3.Cross(dirToReceiver, Vector3.forward) *
                               Random.Range(-distanceToReceiver * _randomizeSideOffset, distanceToReceiver * _randomizeSideOffset);
                
                _ = StartFxAsync(Random.Range(0.0f, _randomizeDelay), startPosition, midPoint, endPosition, _duration, _cancellationTokenSource.Token);
            }
            Destroy(gameObject, _duration + _randomizeDelay);
        }

        private async Task StartFxAsync(float delay, 
            Vector3 startPosition, 
            Vector3 middlePosition, 
            Vector3 endPosition, 
            float time, 
            CancellationToken cancellationToken)
        {

            await AsyncExtensions.WaitForSecondsAsync(delay, cancellationToken);
            var coin = Instantiate(_coinPrefab, transform);

            var timer = 0.0f;
        
            while (timer < time)
            {
                var pathParam = timer / time;
                var f = _movingSpeed.Evaluate(pathParam);
                var pathDerivedParam = f * 2.0f;
                var startDerivedPosition = startPosition;
                var endDerivedPosition = middlePosition;

                if (f >= 0.5f)
                {
                    startDerivedPosition = middlePosition;
                    endDerivedPosition = endPosition;
                    pathDerivedParam = (f - 0.5f) * 2.0f;
                }
                
                coin.transform.position = Vector3.Lerp(startDerivedPosition, endDerivedPosition, pathDerivedParam);
                
                timer += Time.deltaTime;
                
                await Task.Yield();
            }
            Destroy(coin.gameObject);
        }
    }
}