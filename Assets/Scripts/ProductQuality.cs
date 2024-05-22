using System.Collections;
using UnityEngine;

namespace AshLight.BakerySim
{
    [RequireComponent(typeof(Product))]
    public class ProductQuality : MonoBehaviour, IHaveFreshness, IHaveDoneness, IHaveTemperature
    {
        private float _freshness = 1f;
        private float _doneness;
        private float _temperature = 65f;
        
        [SerializeField] 
        [Tooltip("How much freshess the item loses per second.")]
        [Range(0f,1f)]
        private float freshnessDecayRate = 0.001f;
    
        [SerializeField] 
        [Tooltip("The expected temperature of the item. (°F)")]
        private float referenceTemperature = 65f;

        private void Start()
        {
            StartCoroutine(DecayFreshness());
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
    
        private IEnumerator DecayFreshness()
        {
            while (_freshness > 0f)
            {
                yield return new WaitForSeconds(1f);
                _freshness -= freshnessDecayRate;
                if (_freshness < 0f)
                {
                    _freshness = 0f;
                }
            }
        }
    }
}