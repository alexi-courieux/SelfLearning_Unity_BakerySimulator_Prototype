using UnityEngine;


[CreateAssetMenu(fileName = "Customers", menuName = "ScriptableObject/_CustomerDictionary", order = 0)]
public class CustomerDictionarySo : ScriptableObject
{
    public Transform[] customerPrefabs;
    
    public Transform GetRandomCustomerPrefab()
    {
        return customerPrefabs[UnityEngine.Random.Range(0, customerPrefabs.Length)];
    }
}