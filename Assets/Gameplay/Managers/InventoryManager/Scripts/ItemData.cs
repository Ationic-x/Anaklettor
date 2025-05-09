using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New ItemData", menuName = "Item/Item (Generic)", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
    public bool canUse;
    public bool canEquip;
    public bool isStackable = false;
    public int maxStackSize = 99; // Default max stack size
    public int currentStackSize = 0; // Default current stack size  
}