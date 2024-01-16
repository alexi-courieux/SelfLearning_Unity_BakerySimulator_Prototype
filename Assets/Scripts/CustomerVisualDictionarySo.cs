using UnityEngine;


[CreateAssetMenu(fileName = "CustomerVisuals", menuName = "ScriptableObject/_CustomerVisualDictionary", order = 0)]
public class CustomerVisualDictionarySo : ScriptableObject
{
    public Transform[] customerVisualPrefabs;
    
    public Transform GetRandomCustomerVisual()
    {
        return customerVisualPrefabs[Random.Range(0, customerVisualPrefabs.Length)];
    }
}