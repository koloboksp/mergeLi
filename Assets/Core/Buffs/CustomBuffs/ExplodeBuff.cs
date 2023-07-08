using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Buffs
{
    public class ExplodeBuff : Buff
    {
        [SerializeField] private BuffCursor _cursorPrefab;
        [SerializeField] private AffectingBuffArea _areaPrefab;
        [SerializeField] private int _halfSize = 1;

        private BuffCursor _cursorInstance;
        private readonly List<AffectingBuffArea> _affectingAreaInstances = new();
        
        protected override void InnerOnBeginDrag(PointerEventData eventData)
        {
            base.InnerOnBeginDrag(eventData);

            _cursorInstance = Instantiate(_cursorPrefab, _gameProcessor.Scene.Field.View.Root);
            _cursorInstance.transform.position = eventData.position;
            _cursorInstance.transform.localScale = Vector3.one;
            
            var size = GetSize(_halfSize);
            for (int i = 0; i < size * size; i++)
            {
                var affectingBuffArea = Instantiate(_areaPrefab, _gameProcessor.Scene.Field.View.Root);
                _affectingAreaInstances.Add(affectingBuffArea);
                affectingBuffArea.transform.localScale = Vector3.one;
            }
        }

        protected override void InnerOnDrag(PointerEventData eventData)
        {
            base.InnerOnDrag(eventData);

            var pointerPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(eventData.position);
            
            var size = GetSize(_halfSize);
            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                var areaIntPosition = new Vector3Int(pointerPosition.x + x - _halfSize, pointerPosition.y + y - _halfSize, 0);
                _affectingAreaInstances[x + size * y].transform.position = _gameProcessor.Scene.Field.GetPointPosition(areaIntPosition);
            }
            
            _cursorInstance.transform.position = eventData.position;
        }

        protected override void InnerOnEndDrag(PointerEventData eventData)
        {
            base.InnerOnEndDrag(eventData);
           
            Destroy(_cursorInstance.gameObject);
            _cursorInstance = null;
            
            foreach (var affectingAreaInstance in _affectingAreaInstances)
                Destroy(affectingAreaInstance.gameObject);
            _affectingAreaInstances.Clear();
            
            var pointerPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(eventData.position);
            var size = GetSize(_halfSize);
            List<Vector3Int> ballsIndexes = new List<Vector3Int>();
            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                ballsIndexes.Add(new Vector3Int(pointerPosition.x + x - _halfSize, pointerPosition.y + y - _halfSize, 0));
            _gameProcessor.Explode(ballsIndexes);
        }

        private static int GetSize(int halfSize)
        {
            return halfSize * 2 + 1;
        }
    }
}