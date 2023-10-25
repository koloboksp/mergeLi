
using UnityEngine;

public class Follower : MonoBehaviour
{
    private static Follower instance = null;

    private static Transform s_target;
    private static bool s_doFollow;

    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        instance = this;
    }

    public static void Follow(Transform target, bool doFollow)
    {
        if (instance == null)
            return;

        s_target = target;
        s_doFollow = doFollow;
    }

    private void Update()
    {
        if (s_doFollow && s_target != null)
            transform.position = s_target.position + offset;
    }
}
