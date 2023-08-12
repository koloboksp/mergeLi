using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Buffs
{
    public class ExplodeBuff : Buff
    {
        private static readonly List<UIGameScreen_BuffArea> _noAllocFoundBuffAreas = new List<UIGameScreen_BuffArea>();
        
        [SerializeField] private BuffCursor _cursorPrefab;
        [SerializeField] private AffectingBuffArea _areaPrefab;
        [SerializeField] private int _halfSize = 1;

        private BuffCursor _cursorInstance;
        private AffectingBuffArea[,] _affectingAreaInstances;
        private readonly List<Vector3Int> _ballsIndexesToExplode = new();
        
        protected override void InnerOnBeginDrag(PointerEventData eventData)
        {
            base.InnerOnBeginDrag(eventData);

            CreateCursor(eventData.position);
            CreateAffectingArea(eventData.position);
        }

        protected override void InnerOnDrag(PointerEventData eventData)
        {
            base.InnerOnDrag(eventData);
            
            _cursorInstance.transform.position = eventData.position;
            
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(
                _gameProcessor.Scene.Field.ScreenPointToWorld(eventData.position));
            var fieldSize = _gameProcessor.Scene.Field.Size;
            var size = GetSize(_halfSize);
            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++)
            {
                var areaGridPosition = new Vector3Int(pointerGridPosition.x + x - _halfSize, pointerGridPosition.y + y - _halfSize, 0);
                _affectingAreaInstances[x, y].transform.position = _gameProcessor.Scene.Field.GetPointPosition(areaGridPosition);
                _affectingAreaInstances[x, y].gameObject.SetActive(IsAreaValid(areaGridPosition, fieldSize));
            }
        }

        protected override bool InnerOnEndDrag(PointerEventData eventData)
        {
            base.InnerOnEndDrag(eventData);
            
            DestroyCursor();
            DestroyAffectingArea();
            
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                eventData.pointerCurrentRaycast.gameObject.GetComponentsInParent(false, _noAllocFoundBuffAreas);
                if (_noAllocFoundBuffAreas.Count > 0)
                    return false;
            }
            
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(
                _gameProcessor.Scene.Field.ScreenPointToWorld(eventData.position));
            var fieldSize = _gameProcessor.Scene.Field.Size;
            var size = GetSize(_halfSize);
            
            _ballsIndexesToExplode.Clear();
            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                var areaGridPosition = new Vector3Int(pointerGridPosition.x + x - _halfSize, pointerGridPosition.y + y - _halfSize, 0);
                if(IsAreaValid(areaGridPosition, fieldSize))
                    _ballsIndexesToExplode.Add(areaGridPosition);
            }
            
            return _ballsIndexesToExplode.Count > 0;
        }

        private void CreateCursor(Vector3 position)
        {
            DestroyCursor();
            
            _cursorInstance = Instantiate(_cursorPrefab, _gameProcessor.Scene.Field.View.Root);
            _cursorInstance.transform.position = position;
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
        
        private void CreateAffectingArea(Vector3 position)
        {
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(position);
            var size = GetSize(_halfSize);
            
            _affectingAreaInstances = new AffectingBuffArea[size, size];
            for (int x = 0; x < size ; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var areaGridPosition = new Vector3Int(pointerGridPosition.x + x - _halfSize, pointerGridPosition.y + y - _halfSize, 0);
                    var affectingBuffArea = Instantiate(_areaPrefab, _gameProcessor.Scene.Field.View.Root);
                    _affectingAreaInstances[x, y] = affectingBuffArea;
                    _affectingAreaInstances[x, y].transform.position = _gameProcessor.Scene.Field.GetPointPosition(areaGridPosition);
                    affectingBuffArea.transform.localScale = Vector3.one;
                }
            }
        }

        private void DestroyAffectingArea()
        {
            foreach (var affectingAreaInstance in _affectingAreaInstances)
                Destroy(affectingAreaInstance.gameObject);
            _affectingAreaInstances = null;
        }
        
        protected override void InnerProcessUsing()
        {
            _gameProcessor.UseExplodeBuff(Cost, _ballsIndexesToExplode);
        }

        private static int GetSize(int halfSize)
        {
            return halfSize * 2 + 1;
        }

        private static bool IsAreaValid(Vector3Int areaGridPosition, Vector2Int fieldSize)
        {
            return areaGridPosition.x >= 0
                             && areaGridPosition.x < fieldSize.x
                             && areaGridPosition.y >= 0
                             && areaGridPosition.y < fieldSize.y;
        }
    }
}