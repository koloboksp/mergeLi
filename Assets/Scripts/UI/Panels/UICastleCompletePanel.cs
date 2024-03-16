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

        private UICastleCompletePanelData _data;
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UICastleCompletePanelData;
            
            _ = Show(Application.exitCancellationToken);
        }

        async Task Show(CancellationToken cancellationToken)
        {
            _speaker.SetActive(false);
            
            var overUIElements = FindObjectsOfType<UIOverCastleCompletePanel>(true)
                .Select(i => (i, i.transform.parent, i.gameObject.activeSelf))
                .ToList();
            foreach (var overUIElementTuple in overUIElements)
            {
                overUIElementTuple.i.transform.SetParent(transform, true);
                overUIElementTuple.i.gameObject.SetActive(overUIElementTuple.activeSelf);
            }

            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            var castleOriginalParent = activeCastle.transform.parent;
            activeCastle.transform.SetParent(_castleAnimationRoot, true);

            _animation.Play(_completeClipPart1.name);
            _fireworks.SetActive(true);
            
            await AsyncExtensions.WaitForSecondsAsync(_completeClipPart1.length, cancellationToken);
            if (_data.BeforeGiveCoins != null)
                await _data.BeforeGiveCoins();
            
            if (_data.DialogTextKey != GuidEx.Empty)
            {
                _speaker.SetActive(true);
                await _speaker.ShowTextAsync(_data.DialogTextKey, cancellationToken);
                await _data.GameProcessor.GiveCoinsEffect.Show(
                    activeCastle.CoinsAfterComplete, 
                    _speaker.IconRoot.transform, 
                    cancellationToken);
            }
            
            _data.GameProcessor.AddCurrency(activeCastle.CoinsAfterComplete);
            
            await AsyncExtensions.WaitForSecondsAsync(3.0f, cancellationToken);
            
            if (_data.BeforeSelectNextCastle != null)
                await _data.BeforeSelectNextCastle();
            var castlePosition = activeCastle.transform.position;
            _data.GameProcessor.SelectNextCastle();
            
            activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            activeCastle.transform.SetParent(_castleAnimationRoot, true);
            activeCastle.transform.position = castlePosition;
            activeCastle.transform.localScale = Vector3.one;
            
            if (_data.AfterSelectNextCastle != null)
            {
                await _data.AfterSelectNextCastle();
            }
            else
            {
                await Task.WhenAny(
                    AsyncExtensions.WaitForSecondsAsync(10.0f, cancellationToken),
                    AsyncHelpers.WaitForClick(_tapButton, cancellationToken));
            }

            _animation.Play(_completeClipPart2.name);
            await AsyncExtensions.WaitForSecondsAsync(_completeClipPart2.length + 2.0f, cancellationToken);
            
            activeCastle.transform.SetParent(castleOriginalParent);

            _data.GameProcessor.ClearUndoSteps();
            
            foreach (var overUIElementTuple in overUIElements)
                overUIElementTuple.i.transform.SetParent(overUIElementTuple.parent, true);
            
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
    }
    
    public class UICastleCompletePanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
        public GuidEx DialogTextKey { get; set; }
        
        public Func<Task> BeforeGiveCoins;
        public Func<Task> BeforeSelectNextCastle;
        public Func<Task> AfterSelectNextCastle;
    }
}