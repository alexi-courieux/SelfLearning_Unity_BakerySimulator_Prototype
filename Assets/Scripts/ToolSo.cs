using UnityEngine;

[CreateAssetMenu(fileName = "tool", menuName = "ScriptableObject/Tool", order = 0)]
public class ToolSo : ScriptableObject
{
    public Transform prefab;
    public string itemName;
}