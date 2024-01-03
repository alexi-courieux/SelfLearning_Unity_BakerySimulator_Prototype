using UnityEngine;
[CreateAssetMenu(fileName = "OvenRecipe_new", menuName = "ScriptableObject/Recipe/Oven")]
public class OvenRecipeSo : ScriptableObject
{
    public HoldableObjectSo input;
    public HoldableObjectSo output;
    public float timeToProcess;
    public bool burnt;
}