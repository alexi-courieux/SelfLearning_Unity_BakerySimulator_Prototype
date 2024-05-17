using UnityEngine;

namespace AshLight.BakerySim
{
    public class Tool : Item
    {
        public ToolSo ToolSo => toolSo;

        [SerializeField] private ToolSo toolSo;
    }
}