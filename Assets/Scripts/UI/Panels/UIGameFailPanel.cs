using System;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using Core.Gameplay;
using Core.Utils;
using Save;
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
        
        [SerializeField] private UICalculatingScoreLabel _score;
        [SerializeField] private UICalculatingScoreLabel _collapsedLinesCount;
        [SerializeField] private UICalculatingScoreLabel _mergedBallsCount;

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
            
            _score.SetData(_data.GameProcessor);
            _collapsedLinesCount.SetData(_data.GameProcessor);
            _mergedBallsCount.SetData(_data.GameProcessor);
            
            _ = ShowWithAnimation();
        }

        private async Task ShowWithAnimation()
        {
            var inputTokenSource = new CancellationTokenSource();
            
            try
            {
                // Prepare
                _nextBtn.gameObject.SetActive(false);
                _kingDialog.SetDialogActive(false);
                
                // Show panel
                _animation.Play(_show.name);
                await AsyncExtensions.WaitForSecondsAsync(_show.length, inputTokenSource.Token, Application.exitCancellationToken);
                
                // Show panel and wait
                _panel.SetActive(true);
                await AsyncExtensions.WaitForSecondsAsync(2.5f, inputTokenSource.Token, Application.exitCancellationToken);
                
                // Show score
                var saveController = ApplicationController.Instance.SaveController;
                await _score.Show(
                    _data.GameProcessor.SessionProcessor.GetScore(),
                    saveController.SaveProgress.BestSessionScore,
                    inputTokenSource.Token);
                
                await AsyncExtensions.WaitForSecondsAsync(0.5f, inputTokenSource.Token, Application.exitCancellationToken);
                await _collapsedLinesCount.Show(
                    _data.GameProcessor.SessionProcessor.GetCollapseLinesCount(),
                    saveController.SaveProgress.BestSessionCollapsedLines,
                    inputTokenSource.Token);
                
                await AsyncExtensions.WaitForSecondsAsync(0.5f, inputTokenSource.Token, Application.exitCancellationToken);
                await _mergedBallsCount.Show(
                    _data.GameProcessor.SessionProcessor.GetMergedBallsCount(),
                    saveController.SaveProgress.BestSessionMergedBalls,
                    inputTokenSource.Token);
                
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
    }

    public class UIGameFailPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}