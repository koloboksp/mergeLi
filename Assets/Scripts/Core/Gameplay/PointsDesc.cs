using System;
using UnityEngine.Serialization;

namespace Core.Gameplay
{
    [Serializable]
    public class PointsDesc
    {
        [FormerlySerializedAs("BasePoints")] public int Points;
        public int ExtraPoints;
        public int HatPoints;

        public PointsDesc(int points, int extraPoints, int hatPoints)
        {
            Points = points;
            ExtraPoints = extraPoints;
            HatPoints = hatPoints;
        }

        public int Sum()
        {
            return Points + ExtraPoints + HatPoints;
        }

        public void Add(PointsDesc other)
        {
            Points += other.Points;
            ExtraPoints += other.ExtraPoints;
            HatPoints += other.HatPoints;
        }
    }
}