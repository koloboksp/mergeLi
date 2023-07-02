using UnityEngine;

public class UndoBuff : Buff
{
    protected override void OnClick()
    {
        _gameProcessor.Undo();
    }
}