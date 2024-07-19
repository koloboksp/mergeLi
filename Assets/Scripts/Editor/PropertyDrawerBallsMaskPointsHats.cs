using Core.Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(BallsMaskPointsHats))]
public class PropertyDrawerBallsMaskPointsHats : PropertyDrawerBallsMask
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return VisualElement(ViewType.PointsHats, property);
    }
}