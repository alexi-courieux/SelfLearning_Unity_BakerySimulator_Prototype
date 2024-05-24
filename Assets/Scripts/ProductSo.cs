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
        
        public bool haveMarketExtension;
        [Min(0)]
        public float buyPrice;
        [Min(0)]
        public float sellPrice;
        
        public bool haveQualityExtension;
        [Tooltip("How much freshness the item loses per second.")]
        [Range(0f,1f)]
        public float freshnessDecayRate;
        

        public bool CanBeSold()
        {
            return haveMarketExtension && sellPrice > 0f;
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
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(product);
            }
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
            if (!product.haveMarketExtension) return;
            
            GUILayout.BeginVertical("Market Extension", "window");
            
            product.buyPrice = EditorGUILayout.FloatField("Buy Price", product.buyPrice);
            product.sellPrice = EditorGUILayout.FloatField("Sell Price", product.sellPrice);

            if (GUILayout.Button("Remove"))
            {
                product.haveMarketExtension = false;
            }
            
            GUILayout.EndVertical();
        }
        
        private void DrawQualityProperties()
        {
            if (!product.haveQualityExtension) return;
            
            GUILayout.BeginVertical("Quality Extension", "window");
            
            product.freshnessDecayRate = EditorGUILayout.Slider("Freshness Decay Rate", product.freshnessDecayRate, 0f, 1f);

            if (GUILayout.Button("Remove"))
            {
                product.haveQualityExtension = false;
            }
            
            GUILayout.EndVertical();
        }
        
        private void DrawExtensionSelector()
        {
            int selected = 0;
            var options = new List<string> {"-- Select Extension --"};
            if (!product.haveQualityExtension) options.Add("Quality");
            if (!product.haveMarketExtension) options.Add("Market");
            
            selected = EditorGUILayout.Popup("Extension", selected, options.ToArray());
            
            switch (options[selected])
            {
                case "Quality":
                    product.haveQualityExtension = true;
                    break;
                case "Market":
                    product.haveMarketExtension = true;
                    break;
            }
        }
    }
#endif
}