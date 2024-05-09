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
        public abstract void Remove(bool force);
        public abstract void ShowHat(bool activeState);
        public abstract void SetHat(string hatName, string oldHat, bool force);
        public abstract void ChangeUserInactiveHatsFilter();
    }
}