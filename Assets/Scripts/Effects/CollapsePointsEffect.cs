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
        [SerializeField] private PointsEffect _pointsEffect;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private CollapsePointsEffectText _pointsText;
        [SerializeField] private AudioClip _gotClip;

        private DependencyHolder<SoundsPlayer> _soundsPlayer;
        
        public void Run(IReadOnlyList<(List<(BallDesc ball, PointsDesc points)> ballPairs, Vector3 centerPosition)> pointsGroups)
        {
            var receivers = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(i => i.GetComponentsInChildren<IPointsEffectReceiver>())
                .OrderBy(i => i.Priority)
                .ToList();
            
            var originPosition = pointsGroups[0].centerPosition;
            var destinationPosition = receivers[0].Anchor.position;
            
            var sumPoints = new PointsDesc(0, 0, 0);
            foreach (var pointsGroup in pointsGroups)
                foreach (var ballPair in pointsGroup.ballPairs)
                    sumPoints.Add(ballPair.points);
            
            _pointsText.SetPoint(sumPoints);
            var starsCount = CalculateStarsCount(pointsGroups);

            _ = PlayFxAsync(
                originPosition, 
                destinationPosition, 
                sumPoints.Sum(), 
                starsCount,
                receivers,
                Application.exitCancellationToken);
        }

        private static int CalculateStarsCount(IReadOnlyList<(List<(BallDesc ball, PointsDesc points)> ballPairs, Vector3 position)> pointsGroups)
        {
            var startCount = 0;
            for (var groupI = 0; groupI < pointsGroups.Count; groupI++)
            {
                var pointsGroup = pointsGroups[groupI];
                for (var ballI = 0; ballI < pointsGroup.ballPairs.Count; ballI++)
                {
                    var ballPair = pointsGroup.ballPairs[ballI];
                    if (ballI == 0)
                        startCount += (int)Mathf.Log(ballPair.points.Sum(), 2) + 1;
                    else
                        startCount += 1;
                }
            }

            return startCount;
        }

        private async Task PlayFxAsync(
            Vector3 originPosition,
            Vector3 destinationPosition,
            int points, 
            int starsCount, 
            List<IPointsEffectReceiver> receivers, 
            CancellationToken cancellationToken)
        {
            foreach (var receiver in receivers)
                receiver.ReceiveStart(points);
            
            _pointsEffect.Run(starsCount, destinationPosition);
            
            await AsyncExtensions.WaitForSecondsAsync(_duration, cancellationToken);
            
            foreach (var receiver in receivers)
                receiver.Receive(points);
            _soundsPlayer.Value.Play(_gotClip);
            
            foreach (var receiver in receivers)
                receiver.ReceiveFinished();
            
            await AsyncExtensions.WaitForSecondsAsync(_duration, cancellationToken);

            Destroy(gameObject);
        }
    }
}