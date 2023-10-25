
using System.Collections.Generic;
using UnityEngine;

public class BlobTrail : MonoBehaviour
{
    private class Node
    {
        public Vector3 pos;
        public Vector3 dir;
        public float width;
        public float time;

        public Node(Vector3 pos, Vector3 dir, float width, float time)
        {
            this.pos = pos;
            this.dir = dir;
            this.time = time;
            this.width = width;
        }

        public bool Tick()
        {
            time -= Time.deltaTime;
            return time < 0;
        }
    }

    private readonly int[] QUAD = new int[6] { 0, 1, 3, 0, 3, 2 };

    [SerializeField] private Material mat;
    private Material _mat;

    [Space(8)]
    [SerializeField] private AnimationCurve widthCurve =
        new(new Keyframe[2] { new Keyframe(0, 1f), new Keyframe(1f, 1f) });
    [SerializeField] private float width = 2f;

    [Space(8)]
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float step = 1f;

    private List<Node> nodes;

    private Mesh mesh;
    private Vector3[] verts;
    private Vector2[] uvs;
    private int[] tris;
    private int layer;

    private Vector3 oldPos;
    private Vector3 curPos;
    private Vector3 curDir;

    private float phase;

    private static BlobTrail instance;

    private void Awake()
    {
        instance = this;

        layer = gameObject.layer;
        _mat = new Material(mat);
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Start()
    {
        nodes = new List<Node>();

        curPos = transform.position;
        oldPos = curPos;

        verts = new Vector3[0];
        tris = new int[0];
        uvs = new Vector2[0];

        mesh = new Mesh();
    }

    private void Update()
    {
        // Remove old nodes
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].Tick())
                nodes.RemoveAt(i);
        }
        
        // Add new node
        curPos = transform.position;
        
        if (Vector3.SqrMagnitude(curPos - oldPos) > step * step)
        {
            curDir = Vector3.Cross(oldPos - curPos, Vector3.forward).normalized;
            nodes.Add(new Node(curPos, curDir, width, lifeTime));
            oldPos = curPos;

            // correct direction for previous node = smooth corners
            if (nodes.Count > 2)
            {
                nodes[^2].dir = Vector3.Cross(nodes[^3].pos - nodes[^1].pos, Vector3.forward).normalized;
                nodes[^2].width = width * 2f / Vector3.Magnitude(nodes[^2].dir + nodes[^1].dir);
            }
        }

        if (nodes.Count <= 1)
        {
            mesh.Clear();
            return;
        }

        // Update Mesh Size
        if (verts.Length != nodes.Count * 2)
        {
            verts = new Vector3[nodes.Count * 2];
            tris = new int[(nodes.Count - 1) * 6];
            uvs = new Vector2[verts.Length];

            mesh.Clear();
        }

        // Update Mesh Data
        for (int i = 0; i < nodes.Count; i++)
        {
            phase = 1f - 1f * i / nodes.Count;

            curPos = widthCurve.Evaluate(phase) * nodes[i].width * nodes[i].dir;
            verts[i * 2] = nodes[i].pos - curPos;
            verts[i * 2 + 1] = nodes[i].pos + curPos;

            uvs[i * 2] = Vector2.right * phase;
            uvs[i * 2 + 1] = uvs[i * 2] + Vector2.up;
        }

        for (int i = 0; i < nodes.Count - 1; i++)
            for (int j = 0; j < 6; j++)
                tris[i * 6 + j] = i * 2 + QUAD[j];

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        Graphics.DrawMesh(mesh, Matrix4x4.identity, _mat, layer);
    }

    public static void ResetTail()
    {
        if (instance == null)
            return;

        instance.nodes.Clear();
        instance.mesh.Clear();
    }

    public static void SetColor(Color color)
    {
        if (instance != null)
            instance._mat.color = new Color(color.r, color.g, color.b, instance._mat.color.a);
    }
}
