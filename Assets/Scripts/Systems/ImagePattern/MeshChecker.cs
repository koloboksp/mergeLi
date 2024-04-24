using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshChecker : MonoBehaviour
{
    public bool update;
    public Material mat;
    public List<Transform> points;

    private Mesh mesh;
    private Vector3[] verts;

    public RenderTexture rTex;

    private void OnValidate()
    {

        DrawTris();


        // if (mesh == null)
        //     mesh = new Mesh();
        // 
        // if (verts == null || verts.Length != points.Count)
        //     verts = new Vector3[points.Count];
        // 
        // for (int i = 0; i < points.Count; i++)
        //     verts[i] = points[i].position;
        // 
        // mesh.vertices = verts;
        // mesh.triangles = ImagePatternSolver.PolyToTris(verts);
    }

    private void DrawTris()
    {
        RenderTexture.active = rTex;
        
        mat.SetPass(0);

        GL.Clear(true, true, Color.black);
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rTex.width * 2f, rTex.height, 0);

        GL.Begin(GL.TRIANGLES);
        GL.Color(Color.red);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(256, 0, 0);
        GL.Vertex3(128, 256, 0);
        GL.End();

        GL.PopMatrix();
        RenderTexture.active = null;
    }

    private void Update()
    {
        // Graphics.DrawMesh(mesh, Matrix4x4.identity, mat, 0);

       // DrawTris();
    }


}
