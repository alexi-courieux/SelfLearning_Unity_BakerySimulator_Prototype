using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for objects that can display items to customers
/// </summary>
public interface IDisplayItems
{
    public HandleableItemSo[] GetItemsSo();
}
