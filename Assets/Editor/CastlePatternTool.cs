using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CastlePatternTool : EditorWindow
{
    private const int LEFTPANEL_WIDTH = 200;
    private const int HALFPANEL_WIDTH = 100;
    private const int SELDOT_SIZE = 16;
    private const int DEFDOT_SIZE = 10;
    private readonly Color32 SELDOT_COLOR = Color.green;
    private readonly Color32 DEFDOT_COLOR = Color.black;

    private readonly Vector2[] QUAD = new Vector2[] { -Vector2.one, new Vector2(1, -1), Vector2.one, new Vector2(-1, 1) };

    private Texture image;
    private ImagePattern pattern;
    private ImagePattern newPattern;

    [Range(.5f,4f)] private float scale = 1f;

    // private SerializedProperty nodesProp;

    private Vector2 scrollImage;
    private int w, h;
    private Vector2 wh;
    private int selGroupID;
    private int addGroupID;
    private bool isSel;


    private Dictionary<int, List<Vector2>> dict;
    private int dotSize;
    private Color32 dotColor;
    private Color32 defColor;

    private Material mat;
    private Material Mat
    {
        get
        {
            if (mat == null)
                mat = new Material(Shader.Find("Unlit/Color"));
            return mat;
        }
    }

    

    private bool isHoldVert;
    private int selVertID;
    private Vector2 holdPos0;

    [MenuItem("TechArt/Castle Pattern Tool")] 
    static void Init()
    {
        var window = (CastlePatternTool)EditorWindow.GetWindow(typeof(CastlePatternTool));
        window.Show();
    }

    private void OnGUI()
    {
        // Left Tools panel

        GUILayout.BeginVertical(GUILayout.Width(LEFTPANEL_WIDTH));

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
        addGroupID = EditorGUILayout.IntField(addGroupID, GUILayout.Width(HALFPANEL_WIDTH));
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

        GUILayout.BeginArea(new Rect(LEFTPANEL_WIDTH, 0, position.width - LEFTPANEL_WIDTH, position.height));
        if (image != null)
        {
            w = (int)(image.width * scale);
            h = (int)(image.height * scale);
            wh.Set(w, h);

            scrollImage = GUILayout.BeginScrollView(scrollImage);

            // Draw image
            GUILayout.Box(GUIContent.none, GUILayout.Width(w), GUILayout.Height(h));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), image, ScaleMode.StretchToFill);

            DrawLines();

            foreach (var item in dict)
                foreach (var vert in item.Value)
                    DrawVert(Vector2.Scale(vert, wh), Color.red, 10f);

            var e = Event.current;

            if (e.keyCode == KeyCode.Mouse0)
            {
                if (e.type == EventType.MouseDown && !isHoldVert)
                {
                    for (int i = 0; i < dict[selGroupID].Count; i++)
                    {
                        var delta = e.mousePosition - dict[selGroupID][i];
                        if (delta.sqrMagnitude < 30)
                        {
                            isHoldVert = true;
                            holdPos0 = e.mousePosition;
                            selVertID = i;
                            break;
                        }
                    }
                }
                else if (e.type == EventType.MouseUp)
                {
                    isHoldVert = false;
                }
            }

            if (isHoldVert)
            {
                
                
                DrawVert(e.mousePosition, Color.green, 20f);
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

    private void DrawLines()
    {
        Mat.SetPass(0);
        
        // GL.PushMatrix();
        // GL.LoadOrtho();
        
        GL.Begin(GL.LINES);

        Vector3 p0;
        Vector3 p1;

        foreach (var item in dict)
        {
            if (item.Value.Count < 2)
                continue;

            Mat.SetColor("_Color", item.Key == selGroupID ? Color.green : Color.white);

            for (int i = 1; i < item.Value.Count; i++)
            {
                p0 = item.Value[i - 1];
                p1 = item.Value[i];
                
                GL.Vertex(new Vector3(p0.x * w, p0.y * h, 0));
                GL.Vertex(new Vector3(p1.x * w, p1.y * h, 0));
            }
        }

        GL.End();

        // GL.PopMatrix();
    }

    private void DrawVert(Vector2 pos, Color32 col, float size)
    {
        Mat.SetPass(0);
        Mat.SetColor("_Color", col);
        GL.Begin(GL.QUADS);

        for (int i = 0; i < QUAD.Length; i++)
            GL.Vertex(pos + QUAD[i] * size);

        GL.End();
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
