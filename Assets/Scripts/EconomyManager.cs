using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }
    
    public EventHandler<float> OnMoneyChanged;

    private const float StartMoney = 100;
    
    private float _money;

    private void Awake()
    {
        Instance = this;
        _money = PlayerPrefs.GetFloat("Money", StartMoney);
    }
    
    public void AddMoney(float amount)
    {
        _money += amount;
        PlayerPrefs.SetFloat("Money", _money);
        PlayerPrefs.Save();
        OnMoneyChanged?.Invoke(this, _money);
    }
    
    public float GetMoney() => _money;
}