using UnityEngine;

namespace Core.Effects
{
    public class MoveBallEndEffect : MonoBehaviour
    {
        public void Run()
        {
            Destroy(gameObject, 2.0f);
        }
    }
}