using Atom;
using UnityEngine;

public class HatGroup : MonoBehaviour
{
    [SerializeField] private GuidEx _nameKey;

    public GuidEx NameKey => _nameKey;
}