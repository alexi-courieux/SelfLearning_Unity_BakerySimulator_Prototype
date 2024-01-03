using UnityEngine;
[CreateAssetMenu(fileName = "OvenRecipe_new", menuName = "ScriptableObject/Recipe/Oven")]
public class OvenRecipeSo : ScriptableObject
{
    public HandleableItemSo input;
    public HandleableItemSo output;
    public float timeToProcess;
    public bool burnt;
}