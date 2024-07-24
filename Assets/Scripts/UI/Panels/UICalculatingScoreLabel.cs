using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UICalculatingScoreLabel : MonoBehaviour
    {
        [SerializeField] private GameObject _scorePanel;
        [SerializeField] private Transform _scoreLabelRoot;
        [SerializeField] private Transform _scoreIconRoot;
        [SerializeField] public Animation _scoreShowAnimation;
        [SerializeField] private Text _scoreLabel;
        [SerializeField] private AudioClip _countdownClip;
        [SerializeField] private AudioClip _countdownCompleteClip;
        
        [SerializeField] public Animation _newScoreRecordAnimation;
        [SerializeField] private AudioClip _newScoreRecordClip;

        [SerializeField] private AnimationCurve _animationSteps;

        private GameProcessor _gameProcessor;
        
        public void SetData(GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;
            
            _scorePanel.SetActive(false);
            _scoreLabel.text = "--";
            _newScoreRecordAnimation.gameObject.SetActive(false);
        }
        
        public async Task Show(int score, int bestScore, CancellationToken inputToken)
        {
            if (score >= 0)
            {
                _scorePanel.SetActive(true);
                _scoreShowAnimation.Play();
                await AsyncExtensions.WaitForSecondsAsync(_scoreShowAnimation.clip.length, inputToken, Application.exitCancellationToken);

                var sessionScoreLabelScaleEffect = new ScaleEffect(_scoreLabelRoot);
                _ = sessionScoreLabelScaleEffect.Execute(inputToken, Application.exitCancellationToken);
                var sessionScoreIconScaleEffect = new ScaleEffect(_scoreIconRoot);
                _ = sessionScoreIconScaleEffect.Execute(inputToken, Application.exitCancellationToken);
                var sessionScoreCalculationEffect = new CalculationEffect(0, score, _animationSteps,
                    (step, count) =>
                    {
                        var value = Mathf.Lerp(0, score, (float)(step + 1) / (float)(count));
                        value = Mathf.RoundToInt(value);
                        if (step < count - 1)
                            _gameProcessor.SoundsPlayer.Play(_countdownClip);
                        else
                            _gameProcessor.SoundsPlayer.Play(_countdownCompleteClip);

                        sessionScoreLabelScaleEffect.SetMaxScale();
                        sessionScoreIconScaleEffect.SetMaxScale();

                        _scoreLabel.text = $"{value.ToString()}";
                    });
                    
                // Wait for calculation effect
                await sessionScoreCalculationEffect.Execute(inputToken, Application.exitCancellationToken);

                await AsyncExtensions.WaitForSecondsAsync(1.0f, inputToken, Application.exitCancellationToken);

                // Show new best score
                if (score > bestScore)
                {
                    _newScoreRecordAnimation.gameObject.SetActive(true);
                    _newScoreRecordAnimation.Play();

                    _gameProcessor.SoundsPlayer.Play(_newScoreRecordClip);

                    await AsyncExtensions.WaitForSecondsAsync(_newScoreRecordAnimation.clip.length, inputToken, Application.exitCancellationToken);
                }
                else
                {
                    _newScoreRecordAnimation.gameObject.SetActive(false);
                }
            }
        }
        
        private class ScaleEffect
        {
            private float MIN_SCALE_VALUE = 1.0f;
            private float MAX_SCALE_VALUE = 1.2f;
            private float SCALE_SPEED = 1.0f;
            private float SCALE_VALUE;
            
            private readonly Transform _target;
            private bool _autoStop;

            public ScaleEffect(Transform target)
            {
                _target = target;
            }

            public async Task Execute(CancellationToken cancellationToken, CancellationToken exitToken)
            {
                var work = true;
                while (work)
                {
                    exitToken.ThrowIfCancellationRequested();
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _target.localScale = Vector3.one;
                    }
                    else
                    {
                        _target.localScale = Vector3.one * SCALE_VALUE;
                        SCALE_VALUE = Mathf.Clamp(SCALE_VALUE - SCALE_SPEED * Time.deltaTime, MIN_SCALE_VALUE, MAX_SCALE_VALUE);

                        if (_autoStop)
                        {
                            if (SCALE_VALUE <= MIN_SCALE_VALUE)
                            {
                                work = false;
                            }
                        }

                        await Task.Yield();
                    }
                }
            }

            public void SetMaxScale()
            {
                SCALE_VALUE = MAX_SCALE_VALUE;
            }

            public void SetAutoStop()
            {
                _autoStop = true;
            }
        }
        
        private class CalculationEffect
        {
            const int MIN_BEEP_COUNT = 0;
            const int MAX_BEEP_COUNT = 15;
            const float MIN_EFFECT_TIME = 0.0f;
            const float MAX_EFFECT_TIME = 1.5f;

            readonly float _startValue;
            readonly float _stopValue;
            readonly AnimationCurve _stepDependenceFunction;
            readonly Action<int, int> _onStep;

            public CalculationEffect(float startValue, float stopValue, AnimationCurve stepDependenceFunction, Action<int, int> onStep)
            {
                _startValue = startValue;
                _stopValue = stopValue;
                _stepDependenceFunction = stepDependenceFunction;
                _onStep = onStep;
            }

            public async Task Execute(CancellationToken cancellationToken, CancellationToken exitToken)
            {
                var unitCount = Mathf.FloorToInt(_stopValue - _startValue);
                var beepEffectCount = Mathf.Max(MIN_BEEP_COUNT, Mathf.Min(unitCount, MAX_BEEP_COUNT));

                var effectTime = Mathf.Lerp(MIN_EFFECT_TIME, MAX_EFFECT_TIME, (float)(unitCount - MIN_BEEP_COUNT) / (MAX_BEEP_COUNT - MIN_BEEP_COUNT));

                var stepsTemplateStart = _stepDependenceFunction.keys[0].time;
                var stepsTemplateEnd = _stepDependenceFunction.keys[_stepDependenceFunction.length - 1].time;

                var beepEffect = new List<float>();

                if (beepEffectCount >= 2)
                {
                    for (var v = 0; v < beepEffectCount; v++)
                    {
                        float nValue = (float)v / (beepEffectCount - 1);
                        beepEffect.Add(_stepDependenceFunction.Evaluate(stepsTemplateStart + nValue * (stepsTemplateEnd - stepsTemplateStart)));
                    }
                }
                else if (beepEffectCount == 1)
                {
                    beepEffect.Add(_stepDependenceFunction.Evaluate(stepsTemplateStart + 1 * (stepsTemplateEnd - stepsTemplateStart)));
                }
                
                var timer = 0.0f;
                for (var i = 0; i < beepEffect.Count;)
                {
                    exitToken.ThrowIfCancellationRequested();

                    if (cancellationToken.IsCancellationRequested)
                    {
                        _onStep(beepEffect.Count - 1, beepEffect.Count);
                    }
                    else
                    {
                        timer += Time.deltaTime;

                        if (timer > beepEffect[i] * effectTime)
                        {
                            _onStep(i, beepEffect.Count);

                            i++;
                        }

                        await Task.Yield();
                    }
                }
            }
        }

    }
}