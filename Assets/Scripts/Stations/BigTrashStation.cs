using System;
using UnityEngine;

public class BigTrashStation : MonoBehaviour, IInteractable
{
    public EventHandler OnUse;

    public void Interact()
    {
        if (!Player.Instance.HandleSystem.HaveItems()) return;
        Player.Instance.HandleSystem.GetItem().DestroySelf();
        OnUse?.Invoke(this, EventArgs.Empty);
    }
}
