using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class UIStayInRect : MonoBehaviour
    {
        [SerializeField] private RectTransform _targetRoot;

        private static readonly List<UIFxLayer> NoAllocFoundLayers = new();
        private static readonly Vector3[] NoAllocLayerCorners = new Vector3[4];
        private static readonly Vector3[] NoAllocTargetCorners = new Vector3[4];
        
        private void Update()
        {
            GetComponentsInParent(false, NoAllocFoundLayers);
            if (NoAllocFoundLayers.Count == 0)
                return;
            
            NoAllocFoundLayers[0].Root.GetWorldCorners(NoAllocLayerCorners);
            _targetRoot.parent.InverseTransformVectors(NoAllocLayerCorners);
            _targetRoot.GetWorldCorners(NoAllocTargetCorners);
            _targetRoot.parent.InverseTransformVectors(NoAllocTargetCorners);

            var targetLeftBottomCorner = NoAllocTargetCorners[0];
            var targetRightTopCorner = NoAllocTargetCorners[2];
            var layerLeftBottomCorner = NoAllocLayerCorners[0];
            var layerRightTopCorner = NoAllocLayerCorners[2];

            if (targetLeftBottomCorner.x < layerLeftBottomCorner.x)
            {
                var localPosition = _targetRoot.localPosition;
                localPosition.x += layerLeftBottomCorner.x - targetLeftBottomCorner.x;
                _targetRoot.localPosition = localPosition;
            }
            if (targetLeftBottomCorner.y < layerLeftBottomCorner.y)
            {
                var localPosition = _targetRoot.localPosition;
                localPosition.y += layerLeftBottomCorner.y - targetLeftBottomCorner.y;
                _targetRoot.localPosition = localPosition;
            }
            if (targetRightTopCorner.x > layerRightTopCorner.x)
            {
                var localPosition = _targetRoot.localPosition;
                localPosition.x -= (targetRightTopCorner.x - layerRightTopCorner.x);
                _targetRoot.localPosition = localPosition;
            }
            if (targetRightTopCorner.y > layerRightTopCorner.y)
            {
                var localPosition = _targetRoot.localPosition;
                localPosition.y -= (targetRightTopCorner.y - layerRightTopCorner.y);
                _targetRoot.localPosition = localPosition;
            }
        }
    }
}