using UnityEngine;
[CreateAssetMenu(fileName = "BlenderRecipe_new", menuName = "ScriptableObject/Recipe/Blender")]
public class BlenderRecipeSo : ScriptableObject
{
    public HandleableItemSo[] inputs;
    public HandleableItemSo output;
    public float timeToProcess;
}