
using UnityEngine;

[CreateAssetMenu(fileName = "CastleSettings", menuName = "ScriptableObjects/CastleSettings")]

public class CastleViewerPreset : ScriptableObject
{
    [Header("To Make CastleBits")]
    public Shader shVertColor;
    public Shader shMultAlpha;
    public Shader shBlur;
    public Material matCastleBit;

    [Header("Dynamic")]
    public float flipTime = 1f;
    public float glowTime = 2f;
    public AnimationCurve flipCurve;
    public AnimationCurve glowCurve;
    public ParticleSystem fxNextStage;
    public ParticleSystem fxOverCastle;
}
