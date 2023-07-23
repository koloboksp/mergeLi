public abstract class BallSkin : Skin
{
    public abstract bool Selected { set; }
    public abstract bool Moving { set; }
    public abstract int Points { set; }
    public abstract float Transparency { set; }
    public abstract void PathNotFount();
}