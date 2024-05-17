namespace AshLight.BakerySim
{
    /// <summary>
    /// Interface for objects that can display items to customers
    /// </summary>
    public interface IDisplayItems
    {
        public ProductSo[] GetItemsSo();
    }
}
