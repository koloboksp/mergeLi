
using UnityEngine;

[CreateAssetMenu(fileName = "CastleSettings", menuName = "ScriptableObjects/CastleSettings")]

public class CastleSettings : ScriptableObject
{
    public float flipTime = 1f;
    public float glowTime = 2f;

    public AnimationCurve flipCurve;
    public AnimationCurve glowCurve;

    public ParticleSystem fxNextStage;
    public ParticleSystem fxOverCastle;
}
