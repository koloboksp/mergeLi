using Core.Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(BallsMaskPointsHats))]
public class PropertyDrawerBallsMaskPointsHats : PropertyDrawerBallsMask
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var row = new VisualElement();
        row.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        row.Add(CreateLabel(property));
        row.Add(CreateGenerateButton(property));
        row.Add(CreateField(ViewType.PointsHats, property));
        return row;
    }

    private static Button CreateGenerateButton(SerializedProperty property)
    {
        var button = new Button(OnClick);
        button.text = "generate";
        button.style.height = 16;
        return button;
        
        void OnClick()
        {
            var ballInfoMask = property.boxedValue as BallsMask;
            ballInfoMask.Balls.Clear();

            for (int x = 0; x < FieldSize.x; x++)
            {
                for (int y = 0; y < FieldSize.y; y++)
                {
                    ballInfoMask.Balls.Add(
                        new BallDesc(
                            new Vector3Int(x, y, 0),
                            (int)Mathf.Pow(2, Random.Range(0, 8)),
                            null));
                }
            }

            property.boxedValue = ballInfoMask;
            property.serializedObject.ApplyModifiedProperties();
        }

    }
}