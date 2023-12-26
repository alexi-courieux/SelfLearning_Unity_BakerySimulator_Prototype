using UnityEngine;

public class ClearCounter : MonoBehaviour, ICanBeInteracted
{
    public void Interact()
    {
        Debug.Log("Clearing counter");
    }
}
