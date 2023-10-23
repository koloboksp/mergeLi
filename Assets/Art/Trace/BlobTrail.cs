
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
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

    private CanvasRenderer rend;
    [SerializeField] private Material mat;

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

    private Vector3 oldPos;
    private Vector3 curPos;
    private Vector3 curDir;

    private float phase;
    private Matrix4x4 space;

    private static BlobTrail instance;

    private void Awake()
    {
        instance = this;

        rend = GetComponent<CanvasRenderer>();
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

        rend.materialCount = 1;
        rend.SetMaterial(mat, 0);
        rend.SetMesh(mesh);
    }

    void Update()
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
            rend.SetMesh(mesh);
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

        // Convert from World to Local space
        space = transform.worldToLocalMatrix;
        for (int i = 0; i < verts.Length; i++)
            verts[i] = space.MultiplyPoint3x4(verts[i]);

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        rend.SetMesh(mesh);
    }

    public static void ResetTail()
    {
        instance.nodes.Clear();
        instance.mesh.Clear();
    }
}
