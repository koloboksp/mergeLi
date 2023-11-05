using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Buffs
{
    public abstract class AreaEffect : Buff
    {
        private static readonly List<UIGameScreen_BuffArea> _noAllocFoundBuffAreas = new();
        
        [SerializeField] private UIBuffCursor _cursorPrefab;
        [SerializeField] private AffectingBuffArea _areaPrefab;
       
        private UIBuffCursor _cursorInstance;
        private readonly List<AffectingBuffArea> _affectingBuffAreas = new();
        private readonly List<Vector3Int> _affectedAreas = new();

        protected List<Vector3Int> AffectedAreas => _affectedAreas;

        protected override void InnerOnBeginDrag(PointerEventData eventData)
        {
            base.InnerOnBeginDrag(eventData);

            var pointerPosition = _gameProcessor.Scene.Field.ScreenPointToWorld(eventData.position);
            CreateCursor(pointerPosition);
            
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(pointerPosition);
            var affectingArea = GetAffectingArea(pointerGridPosition);
            UpdateAffectingArea(pointerGridPosition, affectingArea);
        }

        protected override void InnerOnDrag(PointerEventData eventData)
        {
            base.InnerOnDrag(eventData);
            var pointerPosition = _gameProcessor.Scene.Field.ScreenPointToWorld(eventData.position);
            _cursorInstance.SetPosition(pointerPosition);
            
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(pointerPosition);
            var affectingArea = GetAffectingArea(pointerGridPosition);
            UpdateAffectingArea(pointerGridPosition, affectingArea);
        }

        protected override bool InnerOnEndDrag(PointerEventData eventData)
        {
            base.InnerOnEndDrag(eventData);
            
            DestroyCursor();
            
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(_gameProcessor.Scene.Field.ScreenPointToWorld(eventData.position));
            _affectedAreas.Clear();
            foreach (var affectingBuffArea in _affectingBuffAreas)
                _affectedAreas.Add(pointerGridPosition + affectingBuffArea.LocalGridPosition);
            
            DestroyAffectingArea();
            
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                eventData.pointerCurrentRaycast.gameObject.GetComponentsInParent(false, _noAllocFoundBuffAreas);
                if (_noAllocFoundBuffAreas.Count > 0)
                    return false;
            }

            if (!CanBuffBeUsed(pointerGridPosition, _affectedAreas))
                return false;
            
            return _affectedAreas.Count > 0;
        }

        protected virtual bool CanBuffBeUsed(Vector3Int pointerGridPosition, IEnumerable<Vector3Int> affectedAreas)
        {
            return true;
        }
        
        protected virtual List<Vector3Int> GetAffectingArea(Vector3Int pointerGridPosition)
        {
            return new List<Vector3Int>() { new(0, 0, 0) };
        }
        
        private void CreateCursor(Vector3 position)
        {
            DestroyCursor();
            
            _cursorInstance = Instantiate(_cursorPrefab, _gameProcessor.UIFxLayer.transform);
            _cursorInstance.SetPosition(position);
            _cursorInstance.transform.localScale = Vector3.one;
        }
        
        private void DestroyCursor()
        {
            if (_cursorInstance != null)
            {
                Destroy(_cursorInstance.gameObject);
                _cursorInstance = null;
            }
        }
        
        private void UpdateAffectingArea(Vector3Int pointerGridPosition, List<Vector3Int> affectingArea)
        {
            for (int i = 0; i < affectingArea.Count ; i++)
            {
                var localGridPosition = affectingArea[i];  
                var affectingBuffArea = _affectingBuffAreas.Find(area => area.LocalGridPosition == localGridPosition);
                if (affectingBuffArea == null)
                {
                    affectingBuffArea = Instantiate(_areaPrefab, _gameProcessor.Scene.Field.View.Root);
                    _affectingBuffAreas.Add(affectingBuffArea);
                    affectingBuffArea.LocalGridPosition = localGridPosition;
                }
                
                affectingBuffArea.transform.position = _gameProcessor.Scene.Field.GetWorldPosition(pointerGridPosition + localGridPosition);
                affectingBuffArea.transform.localScale = Vector3.one;
            }

            for (var index = _affectingBuffAreas.Count - 1; index >= 0; index--)
            {
                var affectingAreaInstance = _affectingBuffAreas[index];
                if (affectingArea.FindIndex(i => i == affectingAreaInstance.LocalGridPosition) < 0)
                {
                    _affectingBuffAreas.RemoveAt(index);
                    Destroy(affectingAreaInstance.gameObject);
                }
            }
        }

        private void DestroyAffectingArea()
        {
            foreach (var affectingAreaInstance in _affectingBuffAreas)
                Destroy(affectingAreaInstance.gameObject);
            _affectingBuffAreas.Clear();
        }
        
        protected static bool IsAreaValid(Vector3Int areaGridPosition, Vector2Int fieldSize)
        {
            return areaGridPosition.x >= 0
                   && areaGridPosition.x < fieldSize.x
                   && areaGridPosition.y >= 0
                   && areaGridPosition.y < fieldSize.y;
        }
    }
}