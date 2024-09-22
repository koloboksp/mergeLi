using UnityEngine;

public class ExampleClass : MonoBehaviour {
    public Mesh mesh;
    public Material mat;

    private void Awake()
    {
        
        mesh = new Mesh();
        var vertices = new Vector3[3];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 1, 0);
        mesh.SetVertices(vertices);
        mesh.UploadMeshData(false);
    }

    public void OnPostRender() {
        // set first shader pass of the material
        mat.SetPass(0);
        // draw mesh at the origin
        Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
    }
}