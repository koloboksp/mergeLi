using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class GiveCoinsEffect : MonoBehaviour
    {
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private AnimationCurve _movingSpeed;
        [SerializeField] private float _randomizeStartPosition = 0.1f;
        [SerializeField] private float _randomizeSideOffset = 0.1f;
        [SerializeField] private float _randomizeDelay = 1.0f;
        [SerializeField] private float _duration = 0.5f;

        private Transform _from;
        private UIGameScreen_Coins _to;

        public async Task Show(int currencyAmount, Transform from, CancellationToken cancellationToken)
        {
            _from = from;
            _to = GameObject.FindObjectOfType<UIGameScreen_Coins>();

            var coinsValue = 5;
            var coinsNum = currencyAmount / coinsValue;
            var restCoinsValue = currencyAmount - coinsValue * coinsNum;

            var splitCoins = new List<int>();
            for (int i = 0; i < coinsNum; i++)
                splitCoins.Add(coinsValue);
            if(restCoinsValue > 0)
                splitCoins.Add(restCoinsValue);
            
            await Run(splitCoins, cancellationToken);
        }
        
        private async Task Run(List<int> splitCoins, CancellationToken cancellationToken)
        {
            
            var list = new List<Task>();
            for (int i = 0; i < splitCoins.Count; i++)
            {
                var startPosition = _from.position + Random.insideUnitSphere * _randomizeStartPosition;
                var endPosition = _to.transform.position;
                var vecToReceiver = endPosition - startPosition;
                var distanceToReceiver = vecToReceiver.magnitude;
                var dirToReceiver = vecToReceiver.normalized;
                var midPoint = startPosition + vecToReceiver * 0.5f +
                               Vector3.Cross(dirToReceiver, Vector3.forward) *
                               Random.Range(-distanceToReceiver * _randomizeSideOffset, distanceToReceiver * _randomizeSideOffset);

                list.Add(StartFx(splitCoins[i], Random.Range(0.0f, _randomizeDelay), startPosition, midPoint, endPosition, _duration, cancellationToken));
            }
            await Task.WhenAll(list);
        }

        private async Task StartFx(
            int coinValue,
            float delay,
            Vector3 startPosition,
            Vector3 middlePosition,
            Vector3 endPosition,
            float time,
            CancellationToken cancellationToken)
        {
           
            await ApplicationController.WaitForSecondsAsync(delay, cancellationToken);
            var coin = Instantiate(_coinPrefab, transform);
            
            var timer = 0.0f;
        
            while (timer < time)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    DestroyCreated();
                    throw new OperationCanceledException();
                }
                
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
            
            _to.Add(coinValue, false);
            
            DestroyCreated();

            void DestroyCreated()
            {
                if (coin != null)
                    Destroy(coin.gameObject);
            }
        }
    }
}