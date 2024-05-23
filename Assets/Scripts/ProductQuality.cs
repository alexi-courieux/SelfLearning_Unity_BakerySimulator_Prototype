using UnityEngine;

namespace AshLight.BakerySim
{
    [RequireComponent(typeof(Product))]
    public class ProductQuality : MonoBehaviour, IHaveFreshness, IHaveDoneness, IHaveTemperature
    {
        [SerializeField] private Product product;
        
        private float _freshness = 1f;
        private float _doneness;
        private float _temperature = 65f;

        private void Update()
        {
            DecayFreshness();
        }

        public float GetFreshness()
        {
            return _freshness;
        }

        public void SetFreshness(float freshness)
        {
            _freshness = freshness;
        }

        public float GetDoneness()
        {
            return _doneness;
        }

        public void SetDoneness(float doneness)
        {
            _doneness = doneness;
        }

        public float GetTemperature()
        {
            return _temperature;
        }

        public void SetTemperature(float temperature)
        {
            _temperature = temperature;
        }
    
        private void DecayFreshness()
        {
            if (_freshness <= 0f) return;
            _freshness -= product.ProductSo.freshnessDecayRate * Time.deltaTime;
        }
    }
}