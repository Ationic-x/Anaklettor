using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Gear Item", menuName = "Item/Gear/Gear Item (Generic)", order = 1)]
public class Gear : ItemData
{
    public int requiredLevel = 1; // Level required to equip the item
    public UnityEvent onEquipEvent; // Event to trigger when the item is used
}