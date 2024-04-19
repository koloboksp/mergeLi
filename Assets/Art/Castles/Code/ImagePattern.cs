using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Image Pattern", menuName = "ScriptableObjects/Image Pattern", order = 1)]
public class ImagePattern : ScriptableObject
{
    public Texture image;
    public List<Vector3> verts; // xy: pos, z: polygon id
}
