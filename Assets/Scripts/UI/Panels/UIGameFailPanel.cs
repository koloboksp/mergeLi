using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using Core.Gameplay;
using Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core
{
    public class UIGameFailPanel : UIPanel
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _show;
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _nextBtn;
        [SerializeField] private UIBubbleDialog _kingDialog;
        [SerializeField] private GuidEx[] _kingDialogKeys;

        
        [SerializeField] private AnimationCurve _animationSteps;
        
        [SerializeField] private GameObject _sessionScorePanel;
        [SerializeField] private Transform _sessionScoreLabelRoot;
        [SerializeField] private Transform _sessionScoreIconRoot;
        [SerializeField] public Animation _scoreShowAnimation;
        [SerializeField] private Text _sessionScoreLabel;
        [SerializeField] private AudioClip _countdownClip;
        [SerializeField] private AudioClip _countdownCompleteClip;
        
        [SerializeField] public Animation _newScoreRecordAnimation;
        [SerializeField] private AudioClip _newScoreRecordClip;
        
        private Model _model;
        private UIGameFailPanelData _data;

        private void Awake()
        {
            _nextBtn.onClick.AddListener(NextBtn_OnClick);
        }
        
        private void NextBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);

            _data = undefinedData as UIGameFailPanelData;
            _model = new Model();
            
            _ = ShowWithAnimation();
        }

        private async Task ShowWithAnimation()
        {
            var inputTokenSource = new CancellationTokenSource();
            
            try
            {
                // Prepare
                _nextBtn.gameObject.SetActive(false);
                _sessionScorePanel.SetActive(false);
                _sessionScoreLabel.text = "--";
                _newScoreRecordAnimation.gameObject.SetActive(false);
                _kingDialog.SetDialogActive(false);
                
                // Show panel
                _animation.Play(_show.name);
                await AsyncExtensions.WaitForSecondsAsync(_show.length, inputTokenSource.Token, Application.exitCancellationToken);
                
                // Show panel and wait
                _panel.SetActive(true);
                await AsyncExtensions.WaitForSecondsAsync(2.5f, inputTokenSource.Token, Application.exitCancellationToken);
                
                // Show score
                var saveController = ApplicationController.Instance.SaveController;
                var sessionScore = _data.GameProcessor.SessionProcessor.GetScore();
                if (sessionScore >= 0)
                {
                    _sessionScorePanel.SetActive(true);
                    _scoreShowAnimation.Play();
                    await AsyncExtensions.WaitForSecondsAsync(_scoreShowAnimation.clip.length, inputTokenSource.Token, Application.exitCancellationToken);

                    var sessionScoreLabelScaleEffect = new ScaleEffect(_sessionScoreLabelRoot);
                    _ = sessionScoreLabelScaleEffect.Execute(inputTokenSource.Token, Application.exitCancellationToken);
                    var sessionScoreIconScaleEffect = new ScaleEffect(_sessionScoreIconRoot);
                    _ = sessionScoreIconScaleEffect.Execute(inputTokenSource.Token, Application.exitCancellationToken);
                    var sessionScoreCalculationEffect = new CalculationEffect(0, sessionScore, _animationSteps,
                        (step, count) =>
                        {
                            var value = Mathf.Lerp(0, sessionScore, (float)(step + 1) / (float)(count));
                            value = Mathf.RoundToInt(value);
                            if (step < count - 1)
                                _data.GameProcessor.SoundsPlayer.Play(_countdownClip);
                            else
                                _data.GameProcessor.SoundsPlayer.Play(_countdownCompleteClip);

                            sessionScoreLabelScaleEffect.SetMaxScale();
                            sessionScoreIconScaleEffect.SetMaxScale();

                            _sessionScoreLabel.text = $"{value.ToString()}";
                        });
                    
                    // Wait for calculation effect
                    await sessionScoreCalculationEffect.Execute(inputTokenSource.Token, Application.exitCancellationToken);

                    await AsyncExtensions.WaitForSecondsAsync(1.0f, inputTokenSource.Token, Application.exitCancellationToken);

                    // Show new best score
                    if (sessionScore > saveController.SaveProgress.BestSessionScore)
                    {
                        _newScoreRecordAnimation.gameObject.SetActive(true);
                        _newScoreRecordAnimation.Play();

                        _data.GameProcessor.SoundsPlayer.Play(_newScoreRecordClip);

                        await AsyncExtensions.WaitForSecondsAsync(_newScoreRecordAnimation.clip.length, inputTokenSource.Token, Application.exitCancellationToken);
                    }
                    else
                    {
                        _newScoreRecordAnimation.gameObject.SetActive(false);
                    }
                }

               
                await AsyncExtensions.WaitForSecondsAsync(1.5f, inputTokenSource.Token, Application.exitCancellationToken);

                // Show a motivating phrase
                var kingDialogKey = _kingDialogKeys[Random.Range(0, _kingDialogKeys.Length)];
                await _kingDialog.ShowTextAsync(kingDialogKey, false, inputTokenSource.Token);
                // Wait for user reads motivational phrase
                await AsyncExtensions.WaitForSecondsAsync(1.5f, inputTokenSource.Token, Application.exitCancellationToken);

                // Show next button
                _nextBtn.gameObject.SetActive(true);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public class Model
        {
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
    }

    public class UIGameFailPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}