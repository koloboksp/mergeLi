
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RampToRend))]
public class RampToRendEditor : Editor
{
    private RampToRend t;

    private DefaultAsset saveFolder;

    private void OnEnable()
    {
        t = (RampToRend)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Extra", "BoldLabel");

        GUILayout.BeginHorizontal();

        saveFolder = (DefaultAsset)EditorGUILayout.ObjectField(GUIContent.none, saveFolder, typeof(DefaultAsset), false);

        if (GUILayout.Button("Save Ramp To Folder"))
            SaveRampMapToFile();

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Clear Rend Prop Block"))
        {
            t.ClearPropertyBlock();
            EditorUtility.SetDirty(t);
        }
    }

    private void SaveRampMapToFile()
    {
        if (saveFolder == null)
            return;

        var tex = t.Tex;
        if (tex == null)
            return;

        tex.wrapMode = TextureWrapMode.Clamp;

        var path = AssetDatabase.GetAssetPath(saveFolder);
        path += "/RampMap_" + Random.Range(0, 9999) + ".asset";
        AssetDatabase.CreateAsset(tex, path);
        AssetDatabase.Refresh();

        // highligth new texture
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
    }


}
