using UnityEngine;

namespace AshLight.BakerySim
{
    public class Product : Item
    {
        public ProductSo ProductSo => productSo;

        [SerializeField] private ProductSo productSo;
    }
}