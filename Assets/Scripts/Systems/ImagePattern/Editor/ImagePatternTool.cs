using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImagePatternTool : EditorWindow
{
    private const int LEFTPANEL_WIDTH = 200;
    private const int HALFPANEL_WIDTH = 100;
    private const int SELDOT_SIZE = 8;
    private const int DEFDOT_SIZE = 4;

    private const float WHEEL_MIN = .5f;
    private const float WHEEL_MAX = 4f;
    private const float WHEEL_INC = -.02f;

    private const float TAP_DIST = .01f; // in normalize space

    private readonly Vector2[] QUAD = new Vector2[] { -Vector2.one, new Vector2(1, -1), Vector2.one, new Vector2(-1, 1) };
    private readonly Vector2 MIDDOT_SIZE = Vector2.one * 10f;
    private readonly Vector2 MIDDOT_HALF = Vector2.one * 5f;
    private readonly Vector2 MASS_SIZE = new(30f, 20f);

    private Texture image;
    private ImagePattern pattern;
    private ImagePattern newPattern;

    [Range(WHEEL_MIN, WHEEL_MAX)] private float scale = 1f;

    private Vector2 scrollImage;
    private int w, h;
    private Vector2 oneToPix; // front weight height
    private Vector2 pixToOne; // back weight height

    private List<List<Vector2>> verts;
    private Vector2[] mass; // centers of mass for each shell in screen pixels
    private int[] tris;

    private Color32 selColor = Color.green;
    private Color32 defColor = Color.gray;

    private bool isDragVert;
    private bool isPanoramic;
    private Vector2 panPos0;

    private int sid;
    private int Sid // selected ShellID
    {
        get
        { 
            return sid; 
        }

        set
        {
            if (value != sid)
            {
                sid = value;
                UpdateTris();
            }
        }
    }

    private int vid; // selected VertexID in selected ShellID

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

    [MenuItem("TechArt/Castle Pattern Tool")] 
    static void Init()
    {
        var window = (ImagePatternTool)EditorWindow.GetWindow(typeof(ImagePatternTool));
        window.Show();
    }

    private void OnGUI()
    {
        // Left Tools panel
        GUILayout.BeginVertical(GUILayout.Width(LEFTPANEL_WIDTH));

        GUILayout.BeginHorizontal();
        newPattern = (ImagePattern)EditorGUILayout.ObjectField(pattern, typeof(ImagePattern), false);

        if (newPattern == null)
        {
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return;
        }

        if (GUILayout.Button("R", GUILayout.Width(20)))
            pattern = null;

        if (newPattern != pattern)
        {
            pattern = newPattern;
            LoadPattern();
        }

        GUILayout.EndHorizontal();

        image = (Texture)EditorGUILayout.ObjectField(image, typeof(Texture), false);
        scale = EditorGUILayout.Slider(scale, WHEEL_MIN, WHEEL_MAX);

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
        GUILayout.Label("Shells", "BoldLabel");

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add"))
            AddShell();
        if (GUILayout.Button("\u2191"))
            MoveShell(true);
        if (GUILayout.Button("\u2193"))
            MoveShell(false);
        GUILayout.Label(" ");
        if (GUILayout.Button("X"))
            DelShell();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        for (int i = 0; i < verts.Count; i++)
        {
            if (GUILayout.Toggle(Sid == i, i.ToString(), "Button"))
                Sid = i;
        }

        GUILayout.EndVertical();

        // Drawing space

        GUILayout.BeginArea(new Rect(LEFTPANEL_WIDTH, 0, position.width - LEFTPANEL_WIDTH, position.height));
        if (image != null)
        {
            UpdateScale();

            scrollImage = GUILayout.BeginScrollView(scrollImage);

            // Draw image
            GUILayout.Box(GUIContent.none, GUILayout.Width(w), GUILayout.Height(h));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), image, ScaleMode.StretchToFill);

            var e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                    
                    if (isDragVert || isPanoramic)
                        break;
                    
                    bool isVert = false;

                    if (verts.Count > 0)
                    {
                        // Tap on Vertex to Select It
                        Vector2 mPos = Vector2.Scale(e.mousePosition, pixToOne);
                        for (int i = 0; i < verts[Sid].Count; i++)
                        {
                            var delta = mPos - verts[Sid][i];
                            if (delta.magnitude < TAP_DIST)
                            {
                                vid = i;

                                if (e.button == 0)
                                {
                                    isDragVert = true;
                                }
                                else if (e.button == 1)
                                {
                                    DelVert();
                                }

                                isVert = true;
                                Repaint();
                                break;
                            }
                        }
                    }

                    if (e.button == 1 && !isVert)
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
                    scale += e.delta.y * WHEEL_INC;
                    scale = Mathf.Clamp(scale, WHEEL_MIN, WHEEL_MAX);
                    UpdateScale();
                    UpdateMass(true);
                    UpdateTris();
                    Repaint();
                    break;
            }

            if (isPanoramic)
            {
                scrollImage = panPos0 - e.mousePosition;
                Repaint();
            }

            // Draw Elements to Work with Verts
            if (verts.Count > 0)
            {
                DrawLines();
                DrawTris();

                // Draw all points for active shell
                for (int i = 0; i < verts[Sid].Count; i++)
                    DrawVert(Vector2.Scale(verts[Sid][i], oneToPix), defColor, DEFDOT_SIZE);

                // Draw active point
                DrawVert(Vector2.Scale(verts[Sid][vid], oneToPix), selColor, SELDOT_SIZE);

                // Draw mass in center of every shell
                for (int i = 0; i < mass.Length; i++)
                    if (GUI.Button(new Rect(mass[i], MASS_SIZE), i.ToString()))
                        Sid = i;

                if (isDragVert)
                {
                    // Move Selected verts
                    verts[Sid][vid] = Vector2.Scale(e.mousePosition, pixToOne);

                    UpdateTris();
                    UpdateMass();
                    Repaint();
                }
                else
                {
                    // Show mid point to insert new vert
                    Vector2 midOnePos;
                    Vector2 midPixPos;
                    int next;

                    for (int i = 0; i < verts[Sid].Count; i++)
                    {
                        next = (i + 1) % verts[Sid].Count;
                        midOnePos = (verts[Sid][i] + verts[Sid][next]) / 2f;
                        midPixPos = Vector2.Scale(midOnePos, oneToPix);

                        if (GUI.Button(new Rect(midPixPos - MIDDOT_HALF, MIDDOT_SIZE), GUIContent.none))
                            verts[Sid].Insert(next, midOnePos);
                    }
                }

                
            }
            
            GUILayout.EndScrollView();
        }
        GUILayout.EndArea();
    }


    private void DrawLines()
    {
        Mat.SetPass(0);

        for (int i = 0; i < verts.Count; i++)
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Sid == i ? selColor : defColor);

            for (int j = 0; j < verts[i].Count; j++)
                GL.Vertex(Vector2.Scale(verts[i][j], oneToPix));

            GL.Vertex(Vector2.Scale(verts[i][0], oneToPix));
            GL.End();
        }
    }

    private void DrawTris()
    {
        Mat.SetPass(0);

        Color col = selColor;
        col.a /= 4f;

        GL.Begin(GL.TRIANGLES);
        GL.Color(col);
        
        for (int i = 0; i < tris.Length; i++)
            GL.Vertex(Vector2.Scale(verts[Sid][tris[i]], oneToPix));
        GL.End();
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
        if (pattern == null)
            return;

        image = pattern.image != null ? pattern.image : Texture2D.whiteTexture;

        verts = ImagePatternSolver.LoadPattern(pattern);

        Sid = 0;
        vid = 0;

        UpdateScale();
        UpdateMass(true);
        UpdateTris();
    }

    private void UpdateTris() => tris = ImagePatternSolver.PolyToTris(verts[Sid].ToArray());

    private void UpdateMass(bool fullUpdate = false)
    {
        if (mass == null || mass.Length != verts.Count)
        {
            mass = new Vector2[verts.Count];
            fullUpdate = true;
        }

        Vector2Int range = fullUpdate ? new Vector2Int(0, verts.Count) : new Vector2Int(Sid, Sid + 1);
        
        for (int i = range.x; i < range.y; i++)
        {
            mass[i] = Vector2.zero;
            for (int j = 0; j < verts[i].Count; j++)
                mass[i] += verts[i][j];
            mass[i] /= verts[i].Count;
            mass[i] = Vector2.Scale(mass[i], oneToPix);
        }
    }

    private void UpdateScale()
    {
        w = (int)(image.width * scale);
        h = (int)(image.height * scale);
        oneToPix.Set(w, h);
        pixToOne.Set(1f / w, 1f / h);
    }

    private void SavePattern()
    {
        if (pattern == null)
            return;

        pattern.image = image;
        pattern.bits = new List<int>();
        pattern.verts = new List<Vector2>();

        int index = 0;
        for (int i = 0; i < verts.Count; i++)
        {
            pattern.bits.Add(index);
            pattern.verts.AddRange(verts[i]);
            index += verts[i].Count;
        }

        EditorUtility.SetDirty(pattern);
        var path = AssetDatabase.GetAssetPath(pattern);
        var guid = AssetDatabase.GUIDFromAssetPath(path);
        AssetDatabase.SaveAssetIfDirty(guid);
        AssetDatabase.Refresh();
    }

    private void AddShell()
    {
        verts.Add(new List<Vector2>() {
            new Vector2(.4f, .4f),
            new Vector2(.4f, .6f),
            new Vector2(.6f, .6f),
            new Vector2(.6f, .4f)
        });

        Sid = verts.Count - 1;
        vid = 0;

        UpdateMass();
        Repaint();
    }

    private void MoveShell(bool moveUp)
    {
        // MoveUp is Move To Zero
        
        if (moveUp && Sid == 0 || !moveUp && Sid == verts.Count - 1)
            return;

        int step = moveUp ? -1 : 1;
        var buf = verts[Sid + step];
        verts[Sid + step] = verts[Sid];
        verts[Sid] = buf;
        Sid += step;

        UpdateMass(true);
        Repaint();
    }

    private void DelShell()
    {
        if (verts.Count == 0)
            return;
        
        if (!EditorUtility.DisplayDialog("Remove shell", "Attention!\nYou can't return this action", "Yes. Remove This Shell"))
            return;

        verts.RemoveAt(Sid);

        Sid = Mathf.Clamp(Sid, 0, verts.Count - 1);
        vid = 0;

        UpdateMass(true);
        Repaint();
    }

    private void DelVert()
    {
        if (verts[Sid].Count < 5)
            return;

        verts[Sid].RemoveAt(vid);
        vid %= verts[Sid].Count;
    }
}
