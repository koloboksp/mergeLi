using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using Core.Gameplay;
using Core.Utils;
using UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UICastleCompletePanel : UIPanel
    {
        [SerializeField] private RectTransform _castleAnimationRoot;
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _completeClipPart1;
        [SerializeField] private AnimationClip _completeClipPart2;
        [SerializeField] private GameObject _fireworks;
        [SerializeField] private UIBubbleDialog _speaker;
        [SerializeField] private UIGameScreen_Coins _coins;
        
        [SerializeField] private UIExtendedButton _tapButton;
        [SerializeField] private AudioClip _completeClip;

        [SerializeField] private float _delayBeforeShowDialog = 2.0f;
        [SerializeField] private float _delayBeforeGiveCoins = 2.0f;
       
        private UICastleCompletePanelData _data;
        
        protected override void InnerActivate()
        {
            base.InnerActivate();
            _coins.MakeSingle();
            _coins.Set(_data.GameProcessor.CurrencyAmount);
        }

        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);
            
            _data = undefinedData as UICastleCompletePanelData;
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency += SaveController_OnConsumeCurrency;

            _ = PlayAsyncSafe(Application.exitCancellationToken);
        }

        protected override void InnerHide()
        {
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency -= SaveController_OnConsumeCurrency;

            base.InnerHide();
        }

        private async Task PlayAsyncSafe(CancellationToken exitToken)
        {
            try
            {
                _speaker.SetActive(false);
                _data.GameProcessor.MusicPlayer.Stop();
                _data.GameProcessor.SoundsPlayer.PlayExclusive(_completeClip);

                var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
                var castleOriginalParent = activeCastle.transform.parent;
                activeCastle.transform.SetParent(_castleAnimationRoot, true);
                var restCastlePoints = activeCastle.RestPoints();
                
                _animation.Play(_completeClipPart1.name);
                _fireworks.SetActive(true);

                await AsyncExtensions.WaitForSecondsAsync(_completeClipPart1.length, exitToken);
                await AsyncExtensions.WaitForSecondsAsync(_delayBeforeShowDialog, exitToken);
                
                if (_data.BeforeGiveCoins != null)
                    await _data.BeforeGiveCoins();
                
                if (_data.ManualGiveCoins)
                {
                    
                }
                else
                {
                    _speaker.SetActive(true);
                
                    if (_data.DialogAfterBuildEndingKey != GuidEx.Empty)
                    {
                        await _speaker.ShowTextAsync(_data.DialogAfterBuildEndingKey, true, exitToken);
                    }
                    _speaker.SetDialogActive(false);

                    await activeCastle.PlayBuildingComplete();
                    
                    await AsyncExtensions.WaitForSecondsAsync(_delayBeforeGiveCoins, exitToken);

                    await _data.GameProcessor.GiveCoinsEffect.Show(
                        activeCastle.CoinsAfterComplete,
                        _speaker.IconRoot.transform.position,
                        exitToken);
                }
                
                await AsyncExtensions.WaitForSecondsAsync(3.0f, exitToken);

                if (_data.BeforeSelectNextCastle != null)
                    await _data.BeforeSelectNextCastle();
                var castlePosition = activeCastle.transform.position;
                _data.GameProcessor.SessionProcessor.SelectNextCastle();
                
                activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
                activeCastle.transform.SetParent(_castleAnimationRoot, true);
                activeCastle.transform.position = castlePosition;
                activeCastle.transform.localScale = Vector3.one;
                activeCastle.SetPoints(restCastlePoints, true);
                
                if (_data.AfterSelectNextCastle != null)
                {
                    await _data.AfterSelectNextCastle();
                }
                else
                {
                    if (_data.DialogOnBuildStartingKey != GuidEx.Empty)
                    {
                        await _speaker.ShowTextAsync(_data.DialogOnBuildStartingKey, true, exitToken);
                        _speaker.SetActive(false);
                    }
                    else
                    {
                        await Task.WhenAny(
                            AsyncExtensions.WaitForSecondsAsync(10.0f, exitToken),
                            AsyncHelpers.WaitForClick(_tapButton, exitToken));

                    }
                }

                _animation.Play(_completeClipPart2.name);
                await AsyncExtensions.WaitForSecondsAsync(_completeClipPart2.length + 2.0f, exitToken);

                activeCastle.transform.SetParent(castleOriginalParent);

                _data.GameProcessor.ClearUndoSteps();

                if (_data.InTheEnd != null)
                {
                    await _data.InTheEnd();
                }

                ApplicationController.Instance.UIPanelController.PopScreen(this);
                _data.GameProcessor.SoundsPlayer.StopPlayExclusive();
                _data.GameProcessor.MusicPlayer.PlayNext();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        private void SaveController_OnConsumeCurrency(int amount)
        {
            _coins.Add(-amount, false);
        }
    }
    
    public class UICastleCompletePanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
        public GuidEx DialogAfterBuildEndingKey { get; set; }
        public GuidEx DialogOnBuildStartingKey { get; set; }
        public bool ManualGiveCoins { get; set; }

        public Func<Task> BeforeGiveCoins;
        public Func<Task> BeforeSelectNextCastle;
        public Func<Task> AfterSelectNextCastle;
        public Func<Task> InTheEnd;
    }
}