using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImagePatternSolver
{
    public static List<List<Vector2>> LoadPattern(ImagePattern pattern)
    {
        var verts = new List<List<Vector2>>();
        int copyStart, copyEnd, copyCount;
        Vector2[] newBit;

        for (int i = 0; i < pattern.bits.Count; i++)
        {
            copyStart = pattern.bits[i];
            copyEnd = i == pattern.bits.Count - 1 ? pattern.verts.Count : pattern.bits[i + 1];
            copyCount = copyEnd - copyStart;

            newBit = new Vector2[copyCount];
            pattern.verts.CopyTo(copyStart, newBit, 0, copyCount);
            verts.Add(new List<Vector2>(newBit));
        }

        return verts;
    }

    public static bool PointInPoly(Vector2 pnt, List<Vector2> poly)
    {
        int nextID = 0;
        int crossCount = 0;
        pnt += Vector2.one * .001f;
        Vector2 viewPoint = pnt + Vector2.right * 2f;

        for (int i = 0; i < poly.Count; i++)
        {
            nextID = (i + 1) % poly.Count;

            if (poly[i].x < pnt.x && poly[nextID].x < pnt.x)
                continue;

            if (LinesIsCross(poly[i], poly[nextID], pnt, viewPoint))
                crossCount++;
        }

        return crossCount % 2 == 1;
    }

    public static bool LinesIsCross(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1)
    {
        Vector2 perp = a1 - a0;
        perp = new Vector2(-perp.y, perp.x);
        bool isCross = Vector2.Dot(b0 - a0, perp) * Vector2.Dot(b1 - a0, perp) < 0;

        if (isCross)
        {
            perp = b1 - b0;
            perp = new Vector2(-perp.y, perp.x);
            isCross = Vector2.Dot(a0 - b0, perp) * Vector2.Dot(a1 - b0, perp) < 0;
        }

        return isCross;
    }

    public static int[] PolyToTris(Vector2[] poly)
    {
        var tris = new List<int>();

        // Check is Loop Right or Left
        int rightIndex = 0;
        Vector3 vecPre = poly[0] - poly[^1];
        Vector3 vecNext;

        for (int i = 1; i < poly.Length; i++)
        {
            vecNext = poly[i] - poly[i - 1];
            rightIndex += Vector3.Cross(vecPre, vecNext).z > 0 ? 1 : -1;
            vecPre = vecNext;
        }
        bool isLoopRight = rightIndex < 0;

        // Debug.Log("Is Loop Right " + isLoopRight);

        int v0, v1, v2; // indices of vids
        var vids = new List<int>();
        var cuts = new List<int>();

        for (int i = 0; i < poly.Length; i++)
        {
            vids.Add(i);
            cuts.Add(i);
            cuts.Add((i + 1) % poly.Length);
        }

        v0 = 0;
        int whileFuse = 1000;
        // string log;

        while(true)
        {
            // log = "All vids: ";
            // foreach (var vid in vids)
            //     log += vid + " ";
            // Debug.Log(log);
            
            if (--whileFuse < 0)
            {
                Debug.Log("While Fuse Error");
                break;
            }

            v1 = (v0 + 1) % vids.Count;
            v2 = (v0 + 2) % vids.Count;

            // Test IsRight
            bool isRight = (Vector3.Cross(
                poly[vids[v1]] - poly[vids[v0]],
                poly[vids[v2]] - poly[vids[v0]]).z > 0);

            if (!isLoopRight)
                isRight = !isRight;

            if (isRight)
            {
                if (vids.Count == 3)
                    break;

                v0 = (v0 + 1) % vids.Count;
                continue;
            }

            // Test Is any vertex inside new triangle
            bool inTriangle = false;
            for (int i = 0; i < poly.Length; i++)
            {
                if (PointInTriangle(poly[i], poly[vids[v0]], poly[vids[v1]], poly[vids[v2]]))
                {
                    inTriangle = true;
                    break;
                }
            }

            if (inTriangle)
            {
                if (vids.Count == 3)
                    break;

                v0 = (v0 + 1) % vids.Count;
                continue;
            }

            tris.Add(vids[v0]);
            tris.Add(vids[v1]);
            tris.Add(vids[v2]);

            if (vids.Count == 3)
                break;

            vids.RemoveAt(v1);
            v0 = (v0 + 1) % vids.Count;
        }

        if (isLoopRight)
            tris.Reverse();

        return tris.ToArray();
    }

    public static bool PointInTriangle(Vector2 pnt, Vector2 t0, Vector2 t1, Vector2 t2)
    {
        //find baricentric coordinates
        var v0 = (t0.x - pnt.x) * (t1.y - t0.y) - (t1.x - t0.x) * (t0.y - pnt.y);
        var v1 = (t1.x - pnt.x) * (t2.y - t1.y) - (t2.x - t1.x) * (t1.y - pnt.y);
        var v2 = (t2.x - pnt.x) * (t0.y - t2.y) - (t0.x - t2.x) * (t2.y - pnt.y);

        //true only if point inside. on edge return false
        return v0 > 0 && v1 > 0 && v2 > 0 || v0 < 0 && v1 < 0 && v2 < 0;
    }
}
