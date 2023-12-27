using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStation : MonoBehaviour, ICanBeInteracted, ICanBeInteractedAlt
{
    public void Interact()
    {
        Debug.Log("Putting item in trash");
    }

    public void InteractAlt()
    {
        Debug.Log("Emptying trash");
    }
}
