using UnityEngine;
[CreateAssetMenu(fileName = "BlenderRecipe_new", menuName = "ScriptableObject/Recipe/Blender")]
public class BlenderRecipeSo : ScriptableObject
{
    public HoldableObjectSo input;
    public HoldableObjectSo output;
    public float timeToProcess;
}