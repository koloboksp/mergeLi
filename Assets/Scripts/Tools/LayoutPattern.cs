
using UnityEngine;

[CreateAssetMenu(fileName = "LayoutPattern", menuName = "ScriptableObjects/LayoutPattern")]
public class LayoutPattern : ScriptableObject
{
    public Font font;
    public Sprite panelImg;
    public Sprite buttonImg;

    [Space(8)]
    public int titleTextSize = 80;
    public int buttonTextSize = 60;
    public Color textColor = Color.white;

    [Space(8)]
    public int textOutlineSize = 4;
    public Color textOutlineColor = new(0, 0, 0, .5f);

    [Space(8)]
    public int panelPadding = 20;
    public int titleHeight = 180;
    public Vector2 buttonSize = new(600, 160);
}
