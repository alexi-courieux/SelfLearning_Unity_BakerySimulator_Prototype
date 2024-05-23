using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AshLight.BakerySim
{
    [CreateAssetMenu(fileName = "Product", menuName = "ScriptableObject/Product")]
    public class ProductSo : ScriptableObject
    {
        public Transform prefab;
        public Sprite sprite;
        public string itemName;
        
        public bool marketExtension;
        [Min(0)]
        public float buyPrice;
        [Min(0)]
        public float sellPrice;
        
        public bool qualityExtension;
        [Tooltip("How much freshness the item loses per second.")]
        [Range(0f,1f)]
        public float freshnessDecayRate;
        

        public bool CanBeSold()
        {
            return sellPrice > 0f;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ProductSo))]
    public class ProductSoEditor : Editor
    {
        ProductSo product;
        public override void OnInspectorGUI()
        {
            product = (ProductSo) target;
            if (product is null) return;
            DrawBaseProperties();
            DrawMarketProperties();
            DrawQualityProperties();
            DrawExtensionSelector();
        }
        
        private void DrawBaseProperties()
        {
            GUILayout.BeginVertical("", "window");
            product.prefab = (Transform) EditorGUILayout.ObjectField("Prefab", product.prefab, typeof(GameObject), false);
            product.sprite = (Sprite) EditorGUILayout.ObjectField("Sprite", product.sprite, typeof(Sprite), false);
            product.itemName = EditorGUILayout.TextField("Item Name", product.itemName);
            GUILayout.EndVertical();
        }
        
        private void DrawMarketProperties()
        {
            if (!product.marketExtension) return;
            
            GUILayout.BeginVertical("Market Extension", "window");
            
            product.buyPrice = EditorGUILayout.FloatField("Buy Price", product.buyPrice);
            product.sellPrice = EditorGUILayout.FloatField("Sell Price", product.sellPrice);

            if (GUILayout.Button("Remove"))
            {
                product.marketExtension = false;
            }
            
            GUILayout.EndVertical();
        }
        
        private void DrawQualityProperties()
        {
            if (!product.qualityExtension) return;
            
            GUILayout.BeginVertical("Quality Extension", "window");
            
            product.freshnessDecayRate = EditorGUILayout.Slider("Freshness Decay Rate", product.freshnessDecayRate, 0f, 1f);

            if (GUILayout.Button("Remove"))
            {
                product.qualityExtension = false;
            }
            
            GUILayout.EndVertical();
        }
        
        private void DrawExtensionSelector()
        {
            int selected = 0;
            var options = new List<string> {"-- Select Extension --"};
            if (!product.qualityExtension) options.Add("Quality");
            if (!product.marketExtension) options.Add("Market");
            
            selected = EditorGUILayout.Popup("Extension", selected, options.ToArray());
            
            switch (options[selected])
            {
                case "Quality":
                    product.qualityExtension = true;
                    break;
                case "Market":
                    product.marketExtension = true;
                    break;
            }
        }
    }
#endif
}