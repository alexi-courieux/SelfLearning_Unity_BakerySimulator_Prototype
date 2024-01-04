using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI moneyText;
    private void Start()
    {
        EconomyManager.Instance.OnMoneyChanged += EconomyManager_OnMoneyChanged;
        UpdateVisuals();
    }

    private void OnDestroy()
    {
        EconomyManager.Instance.OnMoneyChanged -= EconomyManager_OnMoneyChanged;
    }
    
    private void EconomyManager_OnMoneyChanged(object sender, EventArgs e)
    {
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        moneyText.text = EconomyManager.Instance.GetMoney().ToString(CultureInfo.CurrentUICulture);
    }
}
