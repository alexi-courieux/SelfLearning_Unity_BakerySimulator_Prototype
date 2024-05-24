using System.Linq;
using AshLight.BakerySim.Stations;
using UnityEditor;
using UnityEngine;

namespace AshLight.BakerySim
{
    [CreateAssetMenu(fileName = "Recipes", menuName = "ScriptableObject/Recipe/_RecipeDictionary", order = 0)]
    public class RecipesDictionarySo : ScriptableObject
    {
        public OvenRecipeSo[] ovenRecipes;
        public BlenderRecipeSo[] blenderRecipes;
        public ToolRecipeSo[] toolRecipes;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RecipesDictionarySo))]
    public class RecipeDictionarySoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            bool dirty = DrawDefaultInspector();
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

                recipeGuids = AssetDatabase.FindAssets($"t:{nameof(ToolRecipeSo)}");
                recipes.toolRecipes = recipeGuids
                    .Select(guid => AssetDatabase.LoadAssetAtPath<ToolRecipeSo>(AssetDatabase.GUIDToAssetPath(guid)))
                    .ToArray();

                AssetDatabase.SaveAssets();
            }
            
            if (dirty || GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
#endif
}