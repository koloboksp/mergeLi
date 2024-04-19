using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CastlePatternTool : EditorWindow
{
    private const int LEFTPANEL_WIDTH = 200;
    private const int HALFPANEL_WIDTH = 100;
    private const int SELDOT_SIZE = 8;
    private const int DEFDOT_SIZE = 4;


    private const float VIEWSCALE_MIN = .5f;
    private const float VIEWSCALE_MAX = 4f;
    private const float WHEELSCALE = .02f;

    private const float TAP_DIST = .01f; // in normalize space

    private readonly Vector2[] QUAD = new Vector2[] { -Vector2.one, new Vector2(1, -1), Vector2.one, new Vector2(-1, 1) };

    private Texture image;
    private ImagePattern pattern;
    private ImagePattern newPattern;

    [Range(VIEWSCALE_MIN, VIEWSCALE_MAX)] private float scale = 1f;

    // private SerializedProperty nodesProp;

    private Vector2 scrollImage;
    private int w, h;
    private Vector2 wh;
    private Matrix4x4 imgSpace;

    private int selGroupID;
    private int addGroupID;
    private bool isSel;

    private Dictionary<int, List<Vector2>> dict;
    private Color32 selColor = Color.green;
    private Color32 defColor = Color.gray;

    private bool isPanoramic;
    private Vector2 panPos0;

    private bool doRepaint;

    private Material mat;
    private Material Mat
    {
        get
        {
            if (mat == null)
                mat = new Material(Shader.Find("Unlit/VertColor"));
            return mat;
        }
    }

    private bool isDragVert;
    private int selVertID;


    [MenuItem("TechArt/Castle Pattern Tool")] 
    static void Init()
    {
        var window = (CastlePatternTool)EditorWindow.GetWindow(typeof(CastlePatternTool));
        window.Show();
    }

    private void OnGUI()
    {
        // Left Tools panel
        doRepaint = false;

        GUILayout.BeginVertical(GUILayout.Width(LEFTPANEL_WIDTH));

        GUILayout.BeginHorizontal();
        newPattern = (ImagePattern)EditorGUILayout.ObjectField(pattern, typeof(ImagePattern), false);

        if (newPattern == null)
        {
            GUILayout.EndVertical();
            return;
        }

        if (GUILayout.Button("R", GUILayout.Width(20)))
            pattern = null;

        if (newPattern != pattern)
        {
            pattern = newPattern;
            LoadPattern();
            image = pattern.image;
            selGroupID = 0;
        }


        GUILayout.EndHorizontal();

        image = (Texture)EditorGUILayout.ObjectField(image, typeof(Texture), false);
        scale = EditorGUILayout.Slider(scale, VIEWSCALE_MIN, VIEWSCALE_MAX);

        GUILayout.Space(10);
        if (GUILayout.Button("Save Pattern"))
            SavePattern();

        GUILayout.Space(10);
        GUILayout.Label("Colors", "BoldLabel");
        GUILayout.BeginHorizontal();
        defColor = EditorGUILayout.ColorField(defColor);
        selColor = EditorGUILayout.ColorField(selColor);
        GUILayout.EndHorizontal();

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
            imgSpace.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1f / w, 1f / h, 1));

            scrollImage = GUILayout.BeginScrollView(scrollImage);

            // Draw image
            GUILayout.Box(GUIContent.none, GUILayout.Width(w), GUILayout.Height(h));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), image, ScaleMode.StretchToFill);

            var e = Event.current;

            Vector2 mPos; // mousePos in normalized image space

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && !isDragVert && !isPanoramic)
                    {
                        for (int i = 0; i < dict[selGroupID].Count; i++)
                        {
                            mPos = imgSpace.MultiplyPoint3x4(e.mousePosition);
                            var delta = mPos - dict[selGroupID][i];
                            if (delta.magnitude < TAP_DIST)
                            {
                                isDragVert = true;
                                selVertID = i;
                                break;
                            }
                        }
                    }
                    else if (e.button == 1 && !isPanoramic && !isDragVert)
                    {
                        panPos0 = scrollImage - e.mousePosition;
                        isPanoramic = true;
                    }
                    break;

                case EventType.MouseUp:
                    isDragVert = false;
                    isPanoramic = false;
                    break;

                case EventType.ScrollWheel:
                    scale += e.delta.y * WHEELSCALE;
                    scale = Mathf.Clamp(scale, VIEWSCALE_MIN, VIEWSCALE_MAX);
                    Repaint();
                    break;
            }

            if (isPanoramic)
            {
                scrollImage = panPos0 - e.mousePosition;
                Repaint();
            }

            // Draw by GL
            DrawLines();

            foreach (var vert in dict[selGroupID])
                DrawVert(Vector2.Scale(vert, wh), defColor, DEFDOT_SIZE);

            if (isDragVert)
            {
                dict[selGroupID][selVertID] = new Vector2(e.mousePosition.x / w, e.mousePosition.y / h);
                DrawVert(e.mousePosition, selColor, SELDOT_SIZE);
                Repaint();
            }

            

            GUILayout.EndScrollView();
        }
        GUILayout.EndArea();

        // if (doRepaint)
        //     Repaint();
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

            GL.Color(item.Key == selGroupID ? selColor : defColor);

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
        
        GL.Begin(GL.QUADS);
        GL.Color(col);

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
