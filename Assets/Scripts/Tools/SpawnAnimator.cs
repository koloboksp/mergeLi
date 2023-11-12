using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnAnimator : MonoBehaviour
{
    [SerializeField] private AnimationCurve curvePos;
    [SerializeField] private AnimationCurve curveScale;

    [Space(10)]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float posFactor = 1f;
    [SerializeField] private float itemsInterval = .1f;
    
    [SerializeField] private List<Transform> objs;

    private float dur;

    private void Awake()
    {
        dur = Mathf.Max(curvePos.keys[^1].time, curveScale.keys[^1].time);

        foreach (var obj in objs)
            obj.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        for (int i = 0; i < objs.Count; i++)
            Spawn(objs[i], i * itemsInterval);
    }

    private async void Spawn(Transform obj, float delay)
    {
        await Task.Delay((int)(1000 * delay));

        if (!Application.isPlaying)
            return;

        float time = 0;
        Vector3 pos0 = obj.position;

        while (time < dur)
        {
            time += Time.deltaTime;



        }

        

    }


}
