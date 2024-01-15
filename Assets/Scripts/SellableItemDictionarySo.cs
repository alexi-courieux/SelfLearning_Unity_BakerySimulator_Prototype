using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "_SellableItems", menuName = "ScriptableObject/Recipe/_SellableItemDictionary", order = 0)]
public class SellableItemDictionarySo : ScriptableObject
{
    public HandleableItemSo[] sellableItems;
}

[CustomEditor(typeof(SellableItemDictionarySo))]
public class SellableItemDictionarySoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SellableItemDictionarySo itemDictionary = (SellableItemDictionarySo) target;
        if (GUILayout.Button("Autofill"))
        {
            string[] sellableGuids = AssetDatabase.FindAssets($"t:{nameof(HandleableItemSo)}");
            itemDictionary.sellableItems = sellableGuids
                .Select(guid => AssetDatabase.LoadAssetAtPath<HandleableItemSo>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(i => i.CanBeSold())
                .ToArray();
            AssetDatabase.SaveAssets();
        }
    }
}