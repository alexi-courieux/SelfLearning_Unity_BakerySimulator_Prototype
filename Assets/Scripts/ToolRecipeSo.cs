using UnityEngine;

[CreateAssetMenu(fileName = "recipe", menuName = "ScriptableObject/Recipe/Tool")]
public class ToolRecipeSo : ScriptableObject
{
    public ToolSo tool;
    public ProductSo input;
    public ProductSo output;
}
