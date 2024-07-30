using UnityEngine;

namespace Core.Effects
{
    public class MergeBallsEffect : MonoBehaviour
    {
        public void Run()
        {
            Destroy(gameObject, 2.0f);
        }
    }
}