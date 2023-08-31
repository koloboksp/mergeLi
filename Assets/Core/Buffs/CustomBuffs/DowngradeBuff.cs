using System.Collections.Generic;
using UnityEngine;

namespace Core.Buffs
{
    public class DowngradeBuff : AreaEffect
    {
        protected override bool UndoAvailable => true;
        protected override StepTag UndoStepTag => StepTag.UndoDowngrade;
        
        protected override bool CanBuffBeUsed(Vector3Int pointerGridPosition, IEnumerable<Vector3Int> affectedAreas)
        {
            return _gameProcessor.CanGradeAny(affectedAreas);
        }
        
        protected override void InnerProcessUsing()
        {
            _gameProcessor.UseDowngradeBuff(Cost, AffectedAreas, this);
        }
    }
}