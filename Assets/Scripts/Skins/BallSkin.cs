using Core;
using UnityEngine;

namespace Skins
{
    public abstract class BallSkin : Skin
    {
        public abstract void SetData(BallView view);
        public abstract bool Selected { set; }
        public abstract bool Moving { set; }
        public abstract void SetPoints(int points, int oldPoints, bool force);
        public abstract Color MainColor { set; }
        public abstract float Transparency { set; }
        public abstract void PathNotFount();
        public abstract void SetHat(HatView hatView);
        public abstract void Remove(bool force);
        public abstract void ShowHat(bool activeState);
    }
}