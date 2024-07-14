using Core.Gameplay;
using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(BallWeight))]
public class PropertyDrawerBallWeight : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var ballWeight = property.boxedValue as BallWeight;
        var row = new VisualElement();
        row.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        
        var integerFieldPoints = new IntegerField();
        integerFieldPoints.label = nameof(BallWeight.Points);
        integerFieldPoints.style.width = 180;
        integerFieldPoints.userData = new Data { BallWeight = ballWeight, Property = property };
        integerFieldPoints.RegisterValueChangedCallback(IntegerFieldPoints_ValueChanged);
        integerFieldPoints.value = ballWeight.Points;
        row.Add(integerFieldPoints);
       
        var integerFieldWeight = new IntegerField();
        integerFieldWeight.label = nameof(BallWeight.Weight);
        integerFieldWeight.style.width = 180;
        integerFieldWeight.userData = new Data { BallWeight = ballWeight, Property = property };
        integerFieldWeight.RegisterValueChangedCallback(IntegerFieldWeight_ValueChanged);
        integerFieldWeight.value = ballWeight.Weight;
        
        row.Add(integerFieldWeight);
        
        container.Add(row);

        return container;
    }
    
    private void IntegerFieldPoints_ValueChanged(ChangeEvent<int> evt)
    {
        var evtCurrentTarget = evt.currentTarget as IntegerField;
        var userData = evtCurrentTarget.userData as Data;

        var dirty = false;
        
        if (userData.BallWeight.Points != evt.newValue)
        {
            userData.BallWeight.Points = evt.newValue;
            dirty = true;
        }

        if (dirty)
        {
            userData.Property.boxedValue = userData.BallWeight;
            userData.Property.serializedObject.ApplyModifiedProperties();
        }
    }
    
    private void IntegerFieldWeight_ValueChanged(ChangeEvent<int> evt)
    {
        var evtCurrentTarget = evt.currentTarget as IntegerField;
        var userData = evtCurrentTarget.userData as Data;

        var dirty = false;
        
        if (userData.BallWeight.Weight != evt.newValue)
        {
            userData.BallWeight.Weight = evt.newValue;
            dirty = true;
        }

        if (dirty)
        {
            userData.Property.boxedValue = userData.BallWeight;
            userData.Property.serializedObject.ApplyModifiedProperties();
        }
    }
    
    private class Data
    {
        public BallWeight BallWeight;
        public SerializedProperty Property;
    }
}