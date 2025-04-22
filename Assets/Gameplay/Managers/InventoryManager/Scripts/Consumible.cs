using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Consumible Item", menuName = "Item/Consumible Items/Consumible Item (Generic)", order = 1)]
public class Consumible : ItemData
{
    public bool isConsumable = true;
    public UnityEvent onUseEvent; // Event to trigger when the item is used
}