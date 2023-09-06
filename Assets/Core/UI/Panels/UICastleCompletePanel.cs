using System.Collections;
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

        [SerializeField] private Button _tapButton;

        private UICastleCompletePanelData _data;
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UICastleCompletePanelData;
          
            
            StartCoroutine(Show());
        }

        IEnumerator Show()
        {
            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            var castleOriginalParent = activeCastle.transform.parent;
            activeCastle.transform.SetParent(_castleAnimationRoot, true);

            _animation.Play(_completeClipPart1.name);
            _fireworks.SetActive(true);
            
            yield return new WaitForSeconds(_completeClipPart1.length);

            yield return new WaitForSeconds(3.0f);
            var castlePosition = activeCastle.transform.position;
            _data.GameProcessor.SelectNextCastle();
            
            activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            activeCastle.transform.SetParent(_castleAnimationRoot, true);
            activeCastle.transform.position = castlePosition;
            
            yield return new WaitForAny(new WaitForButtonClick(_tapButton), new WaitForSecondsRealtime(10.0f));
            _animation.Play(_completeClipPart2.name);
           
            yield return new WaitForSeconds(_completeClipPart2.length);
            
            activeCastle.transform.SetParent(castleOriginalParent);
        }
        public class UICastleCompletePanelData : UIScreenData
        {
            public GameProcessor GameProcessor { get; set; }
        }
    }
}