using UnityEngine;

public class UndoBuff : Buff
{
    protected override void InnerOnClick()
    {
        _gameProcessor.Undo();
    }
}