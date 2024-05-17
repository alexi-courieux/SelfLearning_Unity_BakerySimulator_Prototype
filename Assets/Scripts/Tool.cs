using UnityEngine;

public class Tool : Item
{
    public ToolSo ToolSo => toolSo;

    [SerializeField] private ToolSo toolSo;
}