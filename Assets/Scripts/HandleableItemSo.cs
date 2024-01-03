using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObject/HoldableObject", order = 0)]
public class HandleableItemSo : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string itemName;
}