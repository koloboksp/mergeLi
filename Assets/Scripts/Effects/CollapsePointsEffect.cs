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
        [SerializeField] private CollapsePointsEffectText _pointsTextPrefab;
        [SerializeField] private AnimationCurve _movingSpeed;
        [SerializeField] private float _randomizeStartPosition = 0.1f;
        [SerializeField] private float _randomizeSideOffset = 0.1f;
        [SerializeField] private float _randomizeDelay = 0.1f;
        [SerializeField] private AudioClip _gotClip;

        private DependencyHolder<SoundsPlayer> _soundsPlayer;
        
        public void Run(IReadOnlyList<(int points, int ballsCount, Vector3 position)> pointsGroups)
        {
            var receivers = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(i => i.GetComponentsInChildren<IPointsEffectReceiver>())
                .OrderBy(i => i.Priority)
                .ToList();
            
            var splitPartPoints = SplitPointsByCoins(pointsGroups);
            
            var fxTasks = new List<Task>();
            var splitOffset = 0;
            for (var index = 0; index < pointsGroups.Count; index++)
            {
                var pointsGroup = pointsGroups[index];
                var pointsText = Instantiate(_pointsTextPrefab, transform, false);
                pointsText.Text = pointsGroup.points.ToString();
                pointsText.gameObject.transform.position = pointsGroup.position;
                
                fxTasks.AddRange(CreateFx(pointsGroup.position, splitPartPoints, splitOffset, pointsGroup.ballsCount, receivers));
                splitOffset += pointsGroup.ballsCount;
            }
            
            var pointsSum = pointsGroups.Sum(i => i.points);
            _ = PlayFxAsync(fxTasks, pointsSum, receivers);
        }

        private async Task PlayFxAsync(List<Task> fxTasks, int pointsSum, List<IPointsEffectReceiver> receivers)
        {
            foreach (var receiver in receivers)
                receiver.ReceiveStart(pointsSum);

            await Task.WhenAll(fxTasks);

            foreach (var receiver in receivers)
                receiver.ReceiveFinished();
            
            Destroy(gameObject);
        }
        
        private static List<int> SplitPointsByCoins(IReadOnlyList<(int points, int ballsCount, Vector3 position)> pointsGroups)
        {
            var pointsSum = pointsGroups.Sum(i => i.points);
            var ballsCountSum = pointsGroups.Sum(i => i.ballsCount);
            var partPoints = pointsSum / ballsCountSum;
            
            var splitPartPoints = new List<int>();
            for (var i = 0; i < ballsCountSum; i++)
                splitPartPoints.Add(i == 0 ? pointsSum : 0);
            
            var restCoinsValue = pointsSum - partPoints * ballsCountSum;
            if (restCoinsValue > 0)
                splitPartPoints.Add(0);
            return splitPartPoints;
        }

        private List<Task> CreateFx(
            Vector3 originPosition,
            IReadOnlyList<int> splitPartPoints,
            int splitOffset,
            int splitCount,
            IReadOnlyList<IPointsEffectReceiver> receivers)
        {
            var fxTasks = new List<Task>();
            var mainReceiver = receivers[0];

            for (var partI = splitOffset; partI < splitCount; partI++)
            {
                Vector3 randDirection = Random.insideUnitCircle;
                var startPosition = originPosition + randDirection * _randomizeStartPosition;
                var endPosition = mainReceiver.Anchor.position;
                var vecToReceiver = endPosition - startPosition;
                var distanceToReceiver = vecToReceiver.magnitude;
                var dirToReceiver = vecToReceiver.normalized;
                var midPoint = startPosition + vecToReceiver * 0.5f +
                               Vector3.Cross(dirToReceiver, Vector3.forward) *
                               Random.Range(-distanceToReceiver * _randomizeSideOffset, distanceToReceiver * _randomizeSideOffset);

                fxTasks.Add(PlayCoinFxAsync(
                    splitPartPoints[partI],
                    partI == 0 ? 0.0f : Random.Range(0.0f, _randomizeDelay),
                    startPosition, midPoint, endPosition,
                    _duration,
                    receivers,
                    Application.exitCancellationToken));
            }

            return fxTasks;
        }
        
        private async Task PlayCoinFxAsync(
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
            catch (OperationCanceledException e)
            {
                
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}