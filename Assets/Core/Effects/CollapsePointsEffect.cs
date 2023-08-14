using System.Collections;
using System.Collections.Generic;
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

        private List<(GameObject item, Vector3 startPosition, Vector3 endPosition, float delay)> _coins = new ();
    
        public float Duration => _duration;
    
        public void Run(int points)
        {
            _points.text = points.ToString();

            var coinsEffectReceiver = GameObject.FindObjectOfType<CoinsEffectReceiver>();

            var coin = GameObject.Instantiate(_coinPrefab, transform);
            _coins.Add((coin, coin.transform.position, coinsEffectReceiver.transform.position, 0.0f));

            StartCoroutine(StartFx(_duration));
        }

        IEnumerator StartFx(float time)
        {
            var timer = 0.0f;
        
            while (timer < time)
            {
                foreach (var coin in _coins)
                {
                    var pathParam = timer / time;
                    coin.item.transform.position = Vector3.Lerp(coin.startPosition, coin.endPosition, _movingSpeed.Evaluate(pathParam));
                }
                timer += Time.deltaTime;
                yield return null;
            }

            foreach (var coin in _coins)
            {
                Destroy(coin.item);
            }
            _coins.Clear();
        
            Destroy(gameObject);
        }
    }
}