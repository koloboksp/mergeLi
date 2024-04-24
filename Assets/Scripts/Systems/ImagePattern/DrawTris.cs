using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTris : MonoBehaviour
{
    // Draws a triangle that covers the middle of the screen
    public Material mat;

    void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.TRIANGLES);

        GL.Color(Color.white);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(0, 1, 0);
        GL.End();
        GL.PopMatrix();
    }
}

