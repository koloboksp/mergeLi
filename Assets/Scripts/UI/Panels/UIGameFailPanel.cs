using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core
{
    
    
    public class UIGameFailPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _nextBtn;
        [SerializeField] private UIBubbleDialog _kingDialog;
        [SerializeField] private GuidEx[] _kingDialogKeys;

        [SerializeField] private UIScore _bestScoreLabel;
        
        private Model _model;
        private UIGameFailPanelData _data;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            _nextBtn.onClick.AddListener(NextBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        private void NextBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIGameFailPanelData;
            _model = new Model();
            
            
        }

        private async Task T()
        {
            CancellationTokenSource _clickScreenCancellationTokenSource = new CancellationTokenSource();
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken, _clickScreenCancellationTokenSource.Token);

            try
            {
                var kingDialogKey = _kingDialogKeys[Random.Range(0, _kingDialogKeys.Length)];
                await _kingDialog.ShowTextAsync(kingDialogKey, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException exception)
            {
                
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