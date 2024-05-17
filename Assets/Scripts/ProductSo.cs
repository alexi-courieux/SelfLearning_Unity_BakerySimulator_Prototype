using UnityEngine;

namespace AshLight.BakerySim
{
    [CreateAssetMenu(fileName = "Product", menuName = "ScriptableObject/Product")]
    public class ProductSo : ScriptableObject
    {
        public Transform prefab;
        public Sprite sprite;
        public string itemName;
        public float buyPrice;
        public float sellPrice;

        public bool CanBeSold()
        {
            return sellPrice > 0f;
        }
    }
}