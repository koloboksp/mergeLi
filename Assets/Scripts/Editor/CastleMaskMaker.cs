
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class CastleMaskMaker : EditorWindow
{
    private readonly float[] BLURCORE = new float[9] { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
    
    private Texture2D texIn;
    private Texture2D texOut;

    private DefaultAsset saveFolder;
    
    [MenuItem("TechArt/CastleMaskMaker")] 
    static void Init()
    {
        var window = (CastleMaskMaker)EditorWindow.GetWindow(typeof(CastleMaskMaker));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Place Mask with Red chanel only");

        texIn = (Texture2D)EditorGUILayout.ObjectField("TexIn", texIn, typeof(Texture2D), false);
        texOut = (Texture2D)EditorGUILayout.ObjectField("TexOut", texOut, typeof(Texture2D), false);

        saveFolder = (DefaultAsset)EditorGUILayout.ObjectField("Save Folder", saveFolder, typeof(DefaultAsset), false);

        if (GUILayout.Button("Convert"))
            Convert();

        if (GUILayout.Button("Save as Asset"))
            SaveToAsset();

        if (GUILayout.Button("Save as PNG"))
            SaveToPNG();
    }

    private void Convert()
    {
        if (texIn == null)
            return;
        
        var cols = texIn.GetPixels32();

        var w = texIn.width;
        var h = texIn.height;

        // define chanels rects
        var spots = new Dictionary<int, List<int>>();

        for (int i = 0; i < cols.Length; i++)
        {
            var val = cols[i].r;
            
            if (val == 0)
                continue;
            
            if (!spots.ContainsKey(val))
            {
                spots.Add(val, new List<int>());
            }
            else
            {
                spots[val].Add(i);
            }
        }


        foreach (var spot in spots)
        {
            // find rect
            var min = new Vector2Int(w, h);
            var max = new Vector2Int(0, 0);

            foreach (var ind in spot.Value)
            {
                var pos = new Vector2Int(ind % w, ind / w);
                min = Vector2Int.Min(min, pos);
                max = Vector2Int.Max(max, pos);
            }

            var center = new Vector2(min.x + max.x, min.y + max.y) / 2f;
            var radius = Vector2.Distance(min, max) / 2f * 1.1f;
            var height = (float)(max.y - min.y);

            foreach (var ind in spot.Value)
            {
                var pos = new Vector2Int(ind % w, ind / w);
                var dist = 1f - Vector2.Distance(center, pos) / radius; // put distance to blue chanel
                var vert = (pos.y - min.y) / height; // put height to green chanel

                cols[ind].g = (byte)(vert * 255);
                cols[ind].b = (byte)(dist * 255);
            }
        }

        // put edge to alpha chanel
        int edgeWidth = 2;
        for (int i = 0; i < cols.Length; i++)
        {
            int posx = i % w;
            int posy = i / w;

            if (posx == 0 || posy == 0 || posx == w - 1 || posy == h - 1)
            {
                cols[i].a = 0;
                continue;
            }

            var colr = cols[i].r;

            bool isEdge = false;
            int lx, ly;

            for (int dx = -edgeWidth; dx <= edgeWidth; dx++)
            {
                for (int dy = -edgeWidth; dy <= edgeWidth; dy++)
                {
                    lx = posx + dx;
                    ly = posy + dy;

                    if (lx < 0 || ly < 0 || lx >= w || ly >= h)
                        continue;

                    if (cols[(posy + dy) * w + posx + dx].r != colr)
                    {
                        isEdge = true;
                        continue;
                    }
                }
                if (isEdge)
                    continue;
            }

            cols[i].a = (byte)(isEdge ? 255 : 0);
        }

        // blur alpha chanel
        var blurCols = new byte[w * h];
        for (int x = 1; x < w - 1; x++)
        {
            for (int y = 1; y < h - 1; y++)
            {
                var sum = 0f;

                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        sum += cols[(y + dy) * w + dx + x].a * BLURCORE[(dy + 1) * 3 + dx + 1];

                blurCols[y * w + x] = (byte)(sum / 16f);
            }
        }
        

        // growup one pixel red chanel
        int growWidth = 2;
        var growCols = new byte[w * h];
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].r != 0)
            {
                growCols[i] = cols[i].r;
                continue;
            }
        
            int posx = i % w;
            int posy = i / w;
        
            bool isGrow = false;
            int lx, ly;
        
            for (int dx = -growWidth; dx <= growWidth; dx++)
            {
                for (int dy = -growWidth; dy <= growWidth; dy++)
                {
                    lx = posx + dx;
                    ly = posy + dy;
        
                    if (lx < 0 || ly < 0 || lx >= w || ly >= h)
                        continue;
        
                    var lookCol = cols[(posy + dy) * w + posx + dx].r;
        
                    if (lookCol != 0)
                    {
                        growCols[i] = lookCol;
                        isGrow = true;
                        continue;
                    }
                }
                if (isGrow)
                    continue;
            }
        }

        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].a = blurCols[i];
            cols[i].r = growCols[i];
        }

        texOut = new Texture2D(w, h, TextureFormat.RGBA32, false, false);
        texOut.SetPixels32(cols);
        texOut.Apply();
    }

    private void SaveToAsset()
    {
        if (texOut == null || saveFolder == null)
            return;

        var folderPath = AssetDatabase.GetAssetPath(saveFolder);

        AssetDatabase.CreateAsset(texOut, folderPath + "/tex_" + Random.Range(1111, 9999) + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SaveToPNG()
    {
        if (texOut == null)
            return;

        var bytes = texOut.EncodeToPNG();
        var folderPath = AssetDatabase.GetAssetPath(saveFolder);

        File.WriteAllBytes(folderPath + "/tex_" + Random.Range(1111, 9999) + ".png", bytes);
    }
}
