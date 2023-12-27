using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/HoldableObject", order = 0)]
public class HoldableObjectSo : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
}