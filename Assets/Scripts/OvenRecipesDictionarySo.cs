using UnityEngine;
[CreateAssetMenu(fileName = "OvenRecipes", menuName = "ScriptableObject/Recipe/OvenRecipeDictionary", order = 0)]
public class OvenRecipesDictionarySo : ScriptableObject
{
    public OvenRecipeSo[] recipes;
}