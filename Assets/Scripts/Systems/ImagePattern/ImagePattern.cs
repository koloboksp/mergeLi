
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Image Pattern", menuName = "ScriptableObjects/Image Pattern", order = 1)]
public class ImagePattern : ScriptableObject
{
    public Texture image;
    public List<int> bits; // index where new bit starts 
    public List<Vector2> verts;
}
