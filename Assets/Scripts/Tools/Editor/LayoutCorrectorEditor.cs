#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayoutCorrector))]
public class LayoutCorrectorEditor : Editor
{
    private LayoutCorrector t;

    private void OnEnable()
    {
        t = (LayoutCorrector)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Correct"))
        {
            t.Correct();
           
        }
    }
}

#endif