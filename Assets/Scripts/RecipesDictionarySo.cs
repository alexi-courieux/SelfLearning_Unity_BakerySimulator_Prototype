using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "ScriptableObject/Recipe/_RecipeDictionary", order = 0)]
public class RecipesDictionarySo : ScriptableObject
{
    public OvenRecipeSo[] ovenRecipes;
    public BlenderRecipeSo[] blenderRecipes;
}

[CustomEditor(typeof(RecipesDictionarySo))]
public class RecipeDictionarySoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RecipesDictionarySo recipes = (RecipesDictionarySo) target;
        if (GUILayout.Button("Autofill"))
        {
            string[] recipeGuids = AssetDatabase.FindAssets($"t:{nameof(OvenRecipeSo)}");
            recipes.ovenRecipes = recipeGuids
                .Select(guid => AssetDatabase.LoadAssetAtPath<OvenRecipeSo>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            recipeGuids = AssetDatabase.FindAssets($"t:{nameof(BlenderRecipeSo)}");
            recipes.blenderRecipes = recipeGuids
                .Select(guid => AssetDatabase.LoadAssetAtPath<BlenderRecipeSo>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
            AssetDatabase.SaveAssets();
        }
    }
}