using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CastlePatternTool : EditorWindow
{
    private const int LEFT_PANEL_WIDTH = 200;
    private const int HALF = 100;

    private Texture image;
    private ImagePattern pattern;
    private ImagePattern newPattern;

    // private List<List<Vector2>> nodes;
    [Range(.5f,4f)] private float scale = 1f;

    // private SerializedProperty nodesProp;

    private Vector2 scrollImage;
    private int w, h;
    private int selGroupID;
    private int addGroupID;
    private bool isSel;

    private Dictionary<int, List<Vector2>> dict;

    [MenuItem("TechArt/Castle Pattern Tool")] 
    static void Init()
    {
        var window = (CastlePatternTool)EditorWindow.GetWindow(typeof(CastlePatternTool));
        window.Show();
    }

    private void OnGUI()
    {
        // Left Tools panel
        
        GUILayout.BeginVertical(GUILayout.Width(LEFT_PANEL_WIDTH));

        newPattern = (ImagePattern)EditorGUILayout.ObjectField(pattern, typeof(ImagePattern), false);

        if (newPattern == null)
        {
            GUILayout.EndVertical();
            return;
        }

        if (newPattern != pattern)
        {
            pattern = newPattern;
            LoadPattern();
            image = pattern.image;
            selGroupID = 0;
        }

        image = (Texture)EditorGUILayout.ObjectField(image, typeof(Texture), false);
        scale = EditorGUILayout.Slider(scale, .5f, 4f);

        GUILayout.Space(10);
        if (GUILayout.Button("Save Pattern"))
            SavePattern();

        GUILayout.Space(10);
        GUILayout.Label("Manage Groups", "BoldLabel");

        GUILayout.BeginHorizontal();
        addGroupID = EditorGUILayout.IntField(addGroupID, GUILayout.Width(HALF));
        if (GUILayout.Button("Add Group"))
            AddGroup();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("Groups", "BoldLabel");
        
        foreach (var item in dict)
        {
            isSel = selGroupID == item.Key;

            if (GUILayout.Toggle(isSel, "ID " + item.Key, "Button"))
                selGroupID = item.Key;
        }

        GUILayout.EndVertical();


        // Drawing space

        GUILayout.BeginArea(new Rect(LEFT_PANEL_WIDTH, 0, position.width - LEFT_PANEL_WIDTH, position.height));
        if (image != null)
        {
            w = (int)(image.width * scale);
            h = (int)(image.height * scale);

            scrollImage = GUILayout.BeginScrollView(scrollImage);

            // Draw image
            GUILayout.Box(GUIContent.none, GUILayout.Width(w), GUILayout.Height(h));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), image, ScaleMode.StretchToFill);

            // Draw shells
            foreach (var item in dict)
                foreach (var vert in item.Value)
                    GUI.Box(new Rect(vert.x * w, vert.y * h, 8, 8), GUIContent.none);

            var e = Event.current;

            if (e.type == EventType.MouseDown && e.keyCode == KeyCode.Mouse0)
            { 
                // TODO Принадлежит ли эта точка кому то из вертексов выделенной группы
            }

            // if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftAlt)
            // {
            //     if (selNodeInd < 0)
            //     {
            //         var mPos = e.mousePosition;
            //         for (int i = 0; i < talk.nodes.Count; i++)
            //         {
            //             if (talk.nodes[i].rect.Contains(mPos))
            //             {
            //                 selNodeInd = i;
            //                 pos0 = e.mousePosition - talk.nodes[i].rect.position;
            //                 break;
            //             }
            //         }
            //     }
            //     else
            //     {
            //         selNodeInd = -1;
            //     }
            // }

            GUILayout.EndScrollView();
        }
        GUILayout.EndArea();
    }

    private void LoadPattern()
    {
        dict = new Dictionary<int, List<Vector2>>();
        int id;
        Vector2 pos;

        for (int i = 0; i < pattern.verts.Count; i++)
        {
            id = (int)pattern.verts[i].z;
            pos = new Vector2(pattern.verts[i].x, pattern.verts[i].y);

            if (dict.ContainsKey(id))
            {
                dict[id].Add(pos);
            }
            else
            {
                dict.Add(id, new List<Vector2>() { pos });
            }
        }
    }
    
    private void SavePattern()
    {
        if (pattern == null || dict == null)
            return;

        pattern.image = image;
        pattern.verts.Clear();

        foreach (var item in dict)
            foreach (var vert in item.Value)
                pattern.verts.Add(new Vector3(vert.x, vert.y, item.Key));

        EditorUtility.SetDirty(pattern);
        var path = AssetDatabase.GetAssetPath(pattern);
        var guid = AssetDatabase.GUIDFromAssetPath(path);
        AssetDatabase.SaveAssetIfDirty(guid);
        AssetDatabase.Refresh();
    }

    private void AddGroup()
    {
        if (dict == null)
            return;

        if (dict.ContainsKey(addGroupID))
            return;

        dict.Add(addGroupID, new List<Vector2>() { new Vector2(.4f, .5f), new Vector2(.6f, .5f) });
    }
}
