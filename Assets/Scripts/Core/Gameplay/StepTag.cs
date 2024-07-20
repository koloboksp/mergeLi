using System;

namespace Core.Gameplay
{
    [Serializable]
    public enum StepTag
    {
        Move,
        Merge,
        Explode1,
        Explode3,
        ExplodeHorizontal,
        ExplodeVertical,
        NextBalls,
        Downgrade,
        Undo,
    
        Select,
        Deselect,
        ChangeSelected,
        NoPath,
    
        UndoMove,
        UndoMerge,
        UndoExplode1,
        UndoExplode3,
        UndoExplodeHorizontal,
        UndoExplodeVertical,
        UndoNextBalls,
        UndoDowngrade,
    
        None,
    }
}