using UnityEngine;

namespace AshLight.BakerySim.Stations
{
    [CreateAssetMenu(fileName = "BlenderRecipe_new", menuName = "ScriptableObject/Recipe/Blender")]
    public class BlenderRecipeSo : ScriptableObject
    {
        public ProductSo[] inputs;
        public ProductSo output;
        public float timeToProcess;
    }
}