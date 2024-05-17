using UnityEngine;

namespace AshLight.BakerySim.Stations
{
    [CreateAssetMenu(fileName = "OvenRecipe_new", menuName = "ScriptableObject/Recipe/Oven")]
    public class OvenRecipeSo : ScriptableObject
    {
        public ProductSo input;
        public ProductSo output;
        public float timeToProcess;
        public bool burnt;
    }
}