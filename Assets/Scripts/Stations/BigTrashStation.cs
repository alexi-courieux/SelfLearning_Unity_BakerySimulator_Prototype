using System;
using UnityEngine;

namespace AshLight.BakerySim.Stations
{
    public class BigTrashStation : MonoBehaviour, IInteractable
    {
        public EventHandler OnUse;

        public void Interact()
        {
            if (!Player.Instance.HandleSystem.HaveItems<Product>()) return;
            Player.Instance.HandleSystem.GetItem().DestroySelf();
            OnUse?.Invoke(this, EventArgs.Empty);
        }
    }
}
