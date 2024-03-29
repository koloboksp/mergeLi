
using System.Threading.Tasks;
using UnityEngine;

public class SpawnAnimator : MonoBehaviour
{
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

        for (int i = 0; i < transform.childCount; i++)
            Spawn(transform.GetChild(i), i * itemsDelay + startDelay, instant);
    }
    
    private async void Spawn(Transform obj, float delay, bool instant)
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
            await Task.Delay((int)(1000 * delay));

            obj.gameObject.SetActive(true);

            while (time < dur)
            {
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
