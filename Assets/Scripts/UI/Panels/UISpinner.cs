using UnityEngine;

namespace Core
{
    public class UISpinner : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Vector3 _speed;
        private void Update()
        {
            var angles = _root.transform.rotation.eulerAngles;
            angles += _speed * Time.deltaTime;
            _root.transform.rotation = Quaternion.Euler(angles);
        }
    }
}