﻿using System.Collections.Generic;
using Atom;
using Core;
using Core.Gameplay;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

[CustomPropertyDrawer(typeof(BallsMask))]
public class PropertyDrawerBallsMask : PropertyDrawer
{
    public enum ViewType
    {
        EnablePoints,
        PointsHats
    }
    
    public static readonly Vector2Int FieldSize = new Vector2Int(9, 9);
    private static readonly Vector2Int CellSize = new Vector2Int(15, 15);
    private const int CellTextSize = 8;
    
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();
        container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        container.Add(CreateLabel(property));
        container.Add(CreateField(ViewType.EnablePoints, property));
        return container;
    }

    protected virtual VisualElement CreateLabel(SerializedProperty property)
    {
        return new Label(property.displayName);
    }
    protected virtual VisualElement CreateField(ViewType viewType, SerializedProperty property)
    {
        var container = new VisualElement();
       
        var ballInfoMask = property.boxedValue as BallsMask;
        
        for (var y = FieldSize.y - 1; y >= 0; y--)
        {
            var toggles = new List<Toggle>();
            var integerFields = new List<IntegerField>();
            var textFields = new List<TextField>();
            
            for (var x = 0; x < FieldSize.x; x++)
            {
                var gridPosition = new Vector3Int(x, y, 0);
                var ballInfo = ballInfoMask.Balls.Find(i => i.GridPosition == gridPosition);

                var toggle = new Toggle();
                var integerField = new IntegerField();
                var textField = new TextField();
                
                var cellUserData = new CellUserData(gridPosition, toggle, integerField, textField, ballInfoMask, property);
                
                toggle.userData = cellUserData;
                toggle.RegisterValueChangedCallback(Toggle_ValueChanged);
                toggle.style.width = CellSize.x;
                toggle.style.height = CellSize.y;
                toggle.value = ballInfo != null;
                toggles.Add(toggle);
                
                integerField.userData = cellUserData;
                integerField.RegisterValueChangedCallback(IntegerField_ValueChanged);
                integerField.style.width = CellSize.x;
                integerField.style.height = CellSize.y;
                integerField.style.fontSize = CellTextSize;
                integerField.style.alignContent = new StyleEnum<Align>(Align.Center);
                integerField.SetEnabled(ballInfo != null);
                integerField.value = ballInfo != null ? ballInfo.Points : -1;

                integerFields.Add(integerField);
                
                if (viewType == ViewType.PointsHats)
                {
                    textField.userData = cellUserData;
                    textField.RegisterValueChangedCallback(TextField_ValueChanged);
                    textField.style.width = CellSize.x;
                    textField.style.height = CellSize.y;
                    textField.SetEnabled(ballInfo != null);
                    textField.value =  ballInfo != null ? ballInfo.HatName : null;
                    textFields.Add(textField);
                }
            }
            
            var row = new VisualElement();
            row.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            foreach (var toggle in toggles)
                row.Add(toggle);
            foreach (var integerField in integerFields)
                row.Add(integerField);
            foreach (var textField in textFields)
                row.Add(textField);

            container.Add(row);
        }

        return container;
    }

    private void Toggle_ValueChanged(ChangeEvent<bool> evt)
    {
        var evtCurrentTarget = evt.currentTarget as Toggle;
        var userData = evtCurrentTarget.userData as CellUserData;

        var dirty = false;

        if (evt.newValue)
        {
            var ballInfo = userData.BallsMask.Balls.Find(i => i.GridPosition == userData.GridPosition);
            if (ballInfo == null)
            {
                userData.BallsMask.Balls.Add(new BallDesc(userData.GridPosition, -1, null));
                userData.BindIntegerField.SetEnabled(true);
                userData.BindIntegerField.SetValueWithoutNotify(-1);
                if (userData.BindTextField != null)
                {
                    userData.BindTextField.SetEnabled(true);
                    userData.BindTextField.SetValueWithoutNotify(string.Empty);
                }
               
                dirty = true;
            }
        }
        else
        {
            if (userData.BallsMask.Balls.RemoveAll(i => i.GridPosition == userData.GridPosition) > 0)
            {
                userData.BindIntegerField.SetEnabled(false);
                userData.BindIntegerField.SetValueWithoutNotify(-1);
                if (userData.BindTextField != null)
                {
                    userData.BindTextField.SetEnabled(false);
                    userData.BindTextField.SetValueWithoutNotify(string.Empty);
                }

                dirty = true;
            }
        }

        if (dirty)
        {
            userData.BallsMaskSerializedProperty.boxedValue = userData.BallsMask;
            userData.BallsMaskSerializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }

    private void IntegerField_ValueChanged(ChangeEvent<int> evt)
    {
        var evtCurrentTarget = evt.currentTarget as IntegerField;
        var userData = evtCurrentTarget.userData as CellUserData;

        var dirty = false;

        var ballInfo = userData.BallsMask.Balls.Find(i => i.GridPosition == userData.GridPosition);
        if (ballInfo != null)
        {
            ballInfo.Points = evt.newValue;
            dirty = true;
        }

        if (dirty)
        {
            userData.BallsMaskSerializedProperty.boxedValue = userData.BallsMask;
            userData.BallsMaskSerializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
    
    private void TextField_ValueChanged(ChangeEvent<string> evt)
    {
        var evtCurrentTarget = evt.currentTarget as TextField;
        var userData = evtCurrentTarget.userData as CellUserData;

        var dirty = false;

        var ballInfo = userData.BallsMask.Balls.Find(i => i.GridPosition == userData.GridPosition);
        if (ballInfo != null)
        {
            ballInfo.HatName = evt.newValue;
            dirty = true;
        }

        if (dirty)
        {
            userData.BallsMaskSerializedProperty.boxedValue = userData.BallsMask;
            userData.BallsMaskSerializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }

    public class CellUserData
    {
        public Vector3Int GridPosition;
        public BallsMask BallsMask;
        public SerializedProperty BallsMaskSerializedProperty;

        public Toggle BindToggle;
        public IntegerField BindIntegerField;
        public TextField BindTextField;
        
        public CellUserData(
            Vector3Int gridPosition,
            Toggle bindToggle,
            IntegerField bindIntegerField,
            TextField bindTextField,
            BallsMask ballsMask, 
            SerializedProperty ballsMaskSerializedProperty)
        {
            GridPosition = gridPosition;
            BindToggle = bindToggle;
            BindIntegerField = bindIntegerField;
            BindTextField = bindTextField;
            BallsMask = ballsMask;
            BallsMaskSerializedProperty = ballsMaskSerializedProperty;
        }
    }
}