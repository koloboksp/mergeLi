
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using UnityEngine;

public class SpawnAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform[] _targets;
    
    [SerializeField] private AnimationCurve curvePos;
    [SerializeField] private AnimationCurve curveScale;
    [SerializeField] private AnimationCurve fullScale;

    [Space(10)]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float posFactor = 1f;
    [SerializeField] private float startDelay = 0f;
    [SerializeField] private float itemsDelay = .1f;

    private float dur;

    public void Play(bool instant)
    {
        dur = Mathf.Max(curvePos.keys[^1].time, curveScale.keys[^1].time, fullScale.keys[^1].time);

        for (var i = 0; i < _targets.Length; i++)
        {
            if (_targets[i].gameObject.activeSelf)
            {
                Spawn(_targets[i], i * itemsDelay + startDelay, instant, Application.exitCancellationToken);
            }
        }
    }
    
    private async void Spawn(Transform obj, float delay, bool instant, CancellationToken exitToken)
    {
        obj.gameObject.SetActive(false);

        float time = 0;
        Vector3 pos0 = obj.localPosition;
        Vector3 scale0 = obj.localScale;
        float sVert;
        float sHorz;

        if (instant)
        {
            obj.gameObject.SetActive(true);
        }
        else
        {
            await AsyncExtensions.WaitForSecondsAsync(delay, Application.exitCancellationToken);

            exitToken.ThrowIfCancellationRequested();
            
            obj.gameObject.SetActive(true);

            while (time < dur)
            {
                exitToken.ThrowIfCancellationRequested();

                time += Time.deltaTime * speed;

                obj.localPosition = pos0 + curvePos.Evaluate(time) * posFactor * Vector3.up;

                sVert = curveScale.Evaluate(time);
                sHorz = Mathf.Sqrt(1 / sVert);
                obj.localScale = fullScale.Evaluate(time) * Vector3.Scale(scale0, new Vector3(sHorz, sVert, sHorz));

                await Task.Yield();

                if (obj == null)
                    return;

                if (!obj.gameObject.activeInHierarchy)
                {
                    obj.localPosition = pos0;
                    obj.localScale = scale0;
                    return;
                }
            }
        }
        sVert = curveScale.Evaluate(dur);
        sHorz = Mathf.Sqrt(1 / sVert);
        obj.localScale = fullScale.Evaluate(dur) * Vector3.Scale(scale0, new Vector3(sHorz, sVert, sHorz)); 
    }
}
