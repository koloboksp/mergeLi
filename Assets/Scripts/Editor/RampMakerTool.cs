using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class RampMakerTool : EditorWindow
{
    private const int SIZE = 128;
    
    private DefaultAsset folder;
    private Material mat;
    private Image image;
    private string propName = "_MainTex";
    private Gradient grad = new Gradient();
    private Texture2D tex;
    private byte[] map;

    [MenuItem("TechArt/Ramp Maker Tool")]
    static void Init()
    {
        var window = (RampMakerTool)EditorWindow.GetWindow(typeof(RampMakerTool));
        window.Show();
    }

    private void OnGUI()
    {
        grad = EditorGUILayout.GradientField("Gradient", grad);
        image = (Image)EditorGUILayout.ObjectField("Image", image, typeof(Image), true);
        mat = (Material)EditorGUILayout.ObjectField("Material", mat, typeof(Material), true);
        propName = EditorGUILayout.TextField("PropName", propName);

        if (GUILayout.Button("Apply"))
            Apply();

        GUILayout.Space(10);
        folder = (DefaultAsset)EditorGUILayout.ObjectField("Folder", folder, typeof(DefaultAsset), true);
        if (GUILayout.Button("Save"))
            Save();
    }

    private void Apply()
    {
        if (map == null || map.Length != SIZE * 4)
            map = new byte[SIZE * 4];

        float fSize = (float)SIZE;
        Color32 col;
        for (int i = 0; i < SIZE; i++)
        {
            col = grad.Evaluate(i / fSize);
            for (int j = 0; j < 4; j++)
                map[i * 4 + j] = col[j];
        }

        if (tex == null || tex.width != SIZE)
            tex = new Texture2D(SIZE, 1, TextureFormat.RGBA32, false, true);

        tex.LoadRawTextureData(map);
        tex.Apply();

        if (image != null)
            mat = image.material;

        if (mat != null)
            mat.SetTexture(propName, tex);
    }

    private void Save()
    {
        if (tex == null || folder == null)
            return;

        var path = AssetDatabase.GetAssetPath(folder);
        AssetDatabase.CreateAsset(tex, path + "/" + Random.Range(111, 999) + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
