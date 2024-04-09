using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        [SerializeField] private AudioClip _gotClip;

        private DependencyHolder<SoundsPlayer> _soundsPlayer;
        
        public float Duration => _duration;

        public void Run(int points, int partsCount)
        {
            _points.text = points.ToString();
            
            var receivers = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(i => i.GetComponentsInChildren<IPointsEffectReceiver>())
                .OrderBy(i => i.Priority)
                .ToList();

            var partPoints = points / partsCount;
            var restCoinsValue = points - partPoints * partsCount;

            var splitPartPoints = new List<int>();
            for (var i = 0; i < partsCount; i++)
                splitPartPoints.Add(partPoints);
            if (restCoinsValue > 0)
                splitPartPoints.Add(restCoinsValue);
            
            _ = StartFxAsync(points, splitPartPoints, receivers);
        }

        private async Task StartFxAsync(
            int points,
            IReadOnlyList<int> splitPartPoints, 
            IReadOnlyList<IPointsEffectReceiver> receivers)
        {
            var _fxTasks = new List<Task>();
            var mainReceiver = receivers[0];

            for (var partI = 0; partI < splitPartPoints.Count; partI++)
            {
                Vector3 randDirection = Random.insideUnitCircle;
                var startPosition = transform.position + randDirection * _randomizeStartPosition;
                var endPosition = mainReceiver.Anchor.position;
                var vecToReceiver = endPosition - startPosition;
                var distanceToReceiver = vecToReceiver.magnitude;
                var dirToReceiver = vecToReceiver.normalized;
                var midPoint = startPosition + vecToReceiver * 0.5f +
                               Vector3.Cross(dirToReceiver, Vector3.forward) *
                               Random.Range(-distanceToReceiver * _randomizeSideOffset, distanceToReceiver * _randomizeSideOffset);

                _fxTasks.Add(StartCoinFxAsync(
                    splitPartPoints[partI],
                    Random.Range(0.0f, _randomizeDelay),
                    startPosition, midPoint, endPosition,
                    _duration,
                    receivers,
                    Application.exitCancellationToken));
            }

            foreach (var receiver in receivers)
                receiver.ReceiveStart(points);
            await Task.WhenAll(_fxTasks);
            foreach (var receiver in receivers)
                receiver.ReceiveFinished();
            
            Destroy(gameObject);
        }
        
        private async Task StartCoinFxAsync(
            int points,
            float delay,
            Vector3 startPosition,
            Vector3 middlePosition,
            Vector3 endPosition,
            float time,
            IReadOnlyList<IPointsEffectReceiver> receivers,
            CancellationToken exitToken)
        {
            try
            {
                await AsyncExtensions.WaitForSecondsAsync(delay, exitToken);
                var coin = Instantiate(_coinPrefab, transform);

                var timer = 0.0f;

                while (timer < time)
                {
                    exitToken.ThrowIfCancellationRequested();

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
            
                foreach (var receiver in receivers)
                    receiver.Receive(points);
                _soundsPlayer.Value.Play(_gotClip);

                Destroy(coin.gameObject);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}