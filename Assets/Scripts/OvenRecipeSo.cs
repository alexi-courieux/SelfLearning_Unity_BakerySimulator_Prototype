using UnityEngine;
[CreateAssetMenu(fileName = "OvenRecipe_new", menuName = "ScriptableObject/Recipe/Oven", order = 0)]
public class OvenRecipeSo : ScriptableObject
{
    public HoldableObjectSo input;
    public HoldableObjectSo output;
    public float timeToProcess;
    public bool burnt;
}