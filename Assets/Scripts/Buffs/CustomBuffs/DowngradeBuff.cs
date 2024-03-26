using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Buffs
{
    public class DowngradeBuff : AreaBuff
    {
        protected virtual bool UndoAvailable => true;
        protected virtual StepTag UndoStepTag => StepTag.UndoDowngrade;
        
        protected override bool CanBuffBeUsed(Vector3Int pointerGridPosition, IEnumerable<Vector3Int> affectedAreas)
        {
            return _gameProcessor.CanDowngradeAny(affectedAreas);
        }
        
        protected override void InnerProcessUsing(PointerEventData pointerEventData)
        {
            _gameProcessor.UseDowngradeBuff(Cost, AffectedAreas, this);
        }

        public override string Id => "Downgrade";
    }
}