using System;
using UnityEngine;

namespace AshLight.BakerySim
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        public EventHandler OnMoneyChanged;

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
            OnMoneyChanged?.Invoke(this, EventArgs.Empty);
        }

        public float GetMoney() => _money;
    }
}