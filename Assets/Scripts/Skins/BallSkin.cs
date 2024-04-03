using Core;
using UnityEngine;

public abstract class BallSkin : Skin
{
    public abstract BallView View { set; }
    public abstract bool Selected { set; }
    public abstract bool Moving { set; }
    public abstract void SetPoints(int points, int oldPoints, bool force);
    public abstract Color MainColor { set; }
    public abstract float Transparency { set; }
    public abstract void PathNotFount();
    public abstract void SetHat(Hat hat);
    public abstract void Remove(bool force);
}