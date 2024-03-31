using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class UIBubbleSpeakEffect : MonoBehaviour
    {
        [SerializeField] private float _scaleValue = 0.05f;
        [SerializeField] private Transform _root;
        [SerializeField] private float _talkSpeed = 3;

        private readonly AnimationCurve _curve = new AnimationCurve();
        private float _timer;
        private float _time;

        private void Awake()
        {
            enabled = false;
        }

        public void Play(string text)
        {
            var textLength = text.Length;
            var words = text.Split(' ');
            var maxWordLength = words.Max(i => i.Length);

            var curveKeyframes = new Keyframe[words.Length * 2];
            var currentWordOffset = 0;

            for (var i = 0; i != words.Length; i++)
            {
                var word = words[i];

                var offset = (float)currentWordOffset / textLength;
                curveKeyframes[i * 2 + 0] = new Keyframe(offset, (float)word.Length / maxWordLength);
                curveKeyframes[i * 2 + 1] = new Keyframe(offset + (float)word.Length * 0.3f / textLength, 0, 0, 0);

                currentWordOffset += word.Length;
            }

            _curve.keys = curveKeyframes;

            _time = words.Length / _talkSpeed;
            _timer = 0;

            enabled = true;

            //  float timeOffset = ((float)(words.Length)) % Talk.AudioSource.clip.length;
            //  float nTimeOffset = timeOffset / Talk.AudioSource.clip.length;
            //  
            //  Talk.AudioSource.timeSamples = (int)(nTimeOffset * (float)Talk.AudioSource.clip.samples);
            //  
            //  Talk.Play(0, 0);
        }

        public void Stop()
        {
            enabled = false;
            _root.localScale = Vector3.one;
            //      Talk.Stop(0, 0);
        }

        private void LateUpdate()
        {
            _timer += Time.deltaTime;
            var nTime = _timer / _time;
            var value = _curve.Evaluate(nTime);

            var scale = (1 + value * _scaleValue);
            //Talk.VolumeScale = scale;
            // Talk.PitchScale = scale;
            _root.localScale = Vector3.one * scale;

            if (!(nTime >= 1))
                return;
            
            _root.localScale = Vector3.one;
            enabled = false;

            // Talk.Stop(0, 0);
        }
    }
}