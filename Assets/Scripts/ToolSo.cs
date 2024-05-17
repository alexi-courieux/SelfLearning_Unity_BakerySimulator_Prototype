using UnityEngine;

[CreateAssetMenu(fileName = "tool", menuName = "ScriptableObject/Tool")]
public class ToolSo : ScriptableObject
{
    public Transform prefab;
    public string itemName;
}