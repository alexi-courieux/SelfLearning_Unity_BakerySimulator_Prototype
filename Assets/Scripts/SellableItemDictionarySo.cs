﻿using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "_SellableItems", menuName = "ScriptableObject/_SellableItemDictionary", order = 0)]
public class SellableItemDictionarySo : ScriptableObject
{
    public ProductSo[] sellableItems;
}

#if UNITY_EDITOR
[CustomEditor(typeof(SellableItemDictionarySo))]
public class SellableItemDictionarySoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SellableItemDictionarySo itemDictionary = (SellableItemDictionarySo) target;
        if (GUILayout.Button("Autofill"))
        {
            string[] sellableGuids = AssetDatabase.FindAssets($"t:{nameof(ProductSo)}");
            itemDictionary.sellableItems = sellableGuids
                .Select(guid => AssetDatabase.LoadAssetAtPath<ProductSo>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(i => i.CanBeSold())
                .ToArray();
            AssetDatabase.SaveAssets();
        }
    }
}
#endif