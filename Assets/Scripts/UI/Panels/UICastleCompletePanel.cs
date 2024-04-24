using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atom;
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
        
        [SerializeField] private Button _tapButton;
        [SerializeField] private AudioClip _completeClip;

        private UICastleCompletePanelData _data;
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UICastleCompletePanelData;
            
            _ = PlayAsyncSafe(Application.exitCancellationToken);
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
                if (_data.BeforeGiveCoins != null)
                    await _data.BeforeGiveCoins();

                

                if (_data.ManualGiveCoins)
                {
                    
                }
                else
                {
                    _speaker.SetActive(true);
                
                    if (_data.DialogTextKey != GuidEx.Empty)
                    {
                        await _speaker.ShowTextAsync(_data.DialogTextKey, exitToken);
                    }
                    
                    await _data.GameProcessor.GiveCoinsEffect.Show(
                        activeCastle.CoinsAfterComplete,
                        _speaker.IconRoot.transform,
                        exitToken);
                
                    _data.GameProcessor.AddCurrency(activeCastle.CoinsAfterComplete);

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
                    await Task.WhenAny(
                        AsyncExtensions.WaitForSecondsAsync(10.0f, exitToken),
                        AsyncHelpers.WaitForClick(_tapButton, exitToken));
                }

                _animation.Play(_completeClipPart2.name);
                await AsyncExtensions.WaitForSecondsAsync(_completeClipPart2.length + 2.0f, exitToken);

                activeCastle.transform.SetParent(castleOriginalParent);

                _data.GameProcessor.ClearUndoSteps();
                
                ApplicationController.Instance.UIPanelController.PopScreen(this);
                _data.GameProcessor.SoundsPlayer.StopPlayExclusive();
                _data.GameProcessor.MusicPlayer.PlayNext();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
    
    public class UICastleCompletePanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
        public GuidEx DialogTextKey { get; set; }
        public bool ManualGiveCoins { get; set; }

        public Func<Task> BeforeGiveCoins;
        public Func<Task> BeforeSelectNextCastle;
        public Func<Task> AfterSelectNextCastle;
    }
}