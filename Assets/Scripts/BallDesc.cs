using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BallDesc
{
    public Vector3Int GridPosition;
    public int Points;
    public int Hat;
    
    public BallDesc()
    {
    }

    public BallDesc(Vector3Int gridPosition, int points, int hat)
    {
        GridPosition = gridPosition;
        Points = points;
        Hat = hat;
    }
    
}

[Serializable]
public class BallsMask
{
    [FormerlySerializedAs("BallInfos")] public List<BallDesc> Balls = new List<BallDesc>();
}