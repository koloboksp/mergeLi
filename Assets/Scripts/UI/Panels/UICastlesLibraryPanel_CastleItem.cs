using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class UICastlesLibraryPanel_CastleItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private UICastlesLibraryPanel_HiddenCastle _hiddenCastlePrefab;
        [SerializeField] private UICastlesLibraryPanel_CastleLabel _castleLabelPrefab;

        private UICastlesLibraryPanel_CastleItemModel _model;
        public RectTransform Root => _root;
        public UICastlesLibraryPanel_CastleItemModel Model => _model;

        public void SetData(
            UICastlesLibraryPanel_CastleItemModel model,
            string lastActiveCastleName,
            int lastActiveCastlePoints,
            float containerWidth)
        {
            _model = model;
            //_root.localScale = Vector3.one;
            //_root.pivot = new Vector2(0, 1);
            var castleContainer = new GameObject($"container", typeof(RectTransform));
            var castleContainerTransform = castleContainer.GetComponent<RectTransform>();
            castleContainerTransform.SetParent(_root);
            castleContainerTransform.localScale = Vector3.one;
            castleContainerTransform.pivot = new Vector2(0, 1);

            var castleViewType = UICastlesLibraryPanel.CastleViewType.Locked;
            var castlePoints = 0;
            var castleCost = 0; 
                
            var saveProgress = ApplicationController.Instance.SaveController.SaveProgress;
            if (saveProgress.IsCastleCompleted(_model.Id))
            {
                castleViewType = UICastlesLibraryPanel.CastleViewType.Completed;
                castlePoints = castleCost;
            }
            else
            {
                if (lastActiveCastleName != null && _model.Id == lastActiveCastleName)
                {
                    castleViewType = UICastlesLibraryPanel.CastleViewType.PartiallyReady;
                    castlePoints = lastActiveCastlePoints;
                }
            }
            
            if (castleViewType == UICastlesLibraryPanel.CastleViewType.Locked)
            {
                var hiddenCastle = Instantiate(_hiddenCastlePrefab, castleContainerTransform);
                hiddenCastle.gameObject.name = _model.Id;
                    
                castleContainerTransform.sizeDelta = _hiddenCastlePrefab.Root.sizeDelta;
            }
            else
            {
                var scaleFactor = 1.0f;
                if (_model.Prefab.Root.sizeDelta.x > containerWidth)
                    scaleFactor = containerWidth / _model.Prefab.Root.sizeDelta.x;
                    
                castleContainerTransform.sizeDelta = _model.Prefab.Root.sizeDelta * new Vector3(scaleFactor, scaleFactor, 1);

                var castle = Instantiate(_model.Prefab, castleContainerTransform);
                castle.gameObject.name = _model.Id;
                castle.SetData(_model.GameProcessor);
                castle.Root.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                if (castleViewType == UICastlesLibraryPanel.CastleViewType.Completed)
                    castle.ShowAsCompleted();
                else
                    castle.SetPoints(lastActiveCastlePoints, true);
                    
                castleCost = castle.GetCost();
            }
            
            var castleLabel = Instantiate(_castleLabelPrefab, _root);
            castleLabel.gameObject.SetActive(true);
            castleLabel.SetData(_model.Prefab.NameKey, castleViewType, castlePoints, castleCost);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
        }
    }
}