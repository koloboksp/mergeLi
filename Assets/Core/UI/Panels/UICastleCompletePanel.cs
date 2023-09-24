using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
        [SerializeField] private RectTransform _kingRoot;
        
        [SerializeField] private Button _tapButton;

        private UICastleCompletePanelData _data;
        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UICastleCompletePanelData;
            
            Show(_cancellationTokenSource.Token);
        }

        async Task Show(CancellationToken cancellationToken)
        {
            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            var castleOriginalParent = activeCastle.transform.parent;
            activeCastle.transform.SetParent(_castleAnimationRoot, true);

            _animation.Play(_completeClipPart1.name);
            _fireworks.SetActive(true);
            
            await ApplicationController.WaitForSecondsAsync(_completeClipPart1.length, cancellationToken);
            if (_data.BeforeGiveCoins != null)
                await _data.BeforeGiveCoins();
            await _data.GameProcessor.GiveCoinsEffect.Show(_kingRoot, cancellationToken);
            await ApplicationController.WaitForSecondsAsync(3.0f, cancellationToken);
            
            var castlePosition = activeCastle.transform.position;
            _data.GameProcessor.SelectNextCastle();
            
            activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            activeCastle.transform.SetParent(_castleAnimationRoot, true);
            activeCastle.transform.position = castlePosition;

          
            await Task.WhenAny(
                ApplicationController.WaitForSecondsAsync(10.0f, cancellationToken), 
                AsyncHelpers.WaitForClick(_tapButton, cancellationToken));
            
            _animation.Play(_completeClipPart2.name);
            await ApplicationController.WaitForSecondsAsync(10.0f, cancellationToken);
            
            activeCastle.transform.SetParent(castleOriginalParent);

            _data.GameProcessor.ClearUndoSteps();
            
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public class UICastleCompletePanelData : UIScreenData
        {
            public GameProcessor GameProcessor { get; set; }
            public Func<Task> BeforeGiveCoins;
        }
    }
}