using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using System;

public class InventoryManagerSingleton : MonoBehaviourSingleton<InventoryManagerSingleton>
{
    [SerializeField] List<ItemData> inventory;

    GameObject[] inventoryCells;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image itemImage;

    protected ItemData selectedItem;
    protected InventoryCellUI selectedCell;
    private Button useButton;
    private Button equipButton;

    int inventoryMax;

    void Awake()
    {
        // inventory = new List<ItemData>();
        inventoryCells = GameObject.FindGameObjectsWithTag("InventoryCellUI");
        inventoryCells = inventoryCells.Reverse().ToArray();
        inventoryMax = inventoryCells.Length - 1;

        Debug.Log($"InventoryManager initialized with {inventoryMax + 1} cells.");
    }

    void Start()
    {
        LoadInventoryData();
        useButton = GameObject.Find("UseButton").GetComponent<Button>();
        equipButton = GameObject.Find("EquipButton").GetComponent<Button>();
    }

    void LoadInventoryData()
    {
        int currentCell = 0;

        foreach (ItemData item in inventory)
        {
            Debug.Log($"Loading item: {item} - {item.GetType()} into inventory cell.");
            // inventoryCells[inventory.IndexOf(item)].GetComponent<InventoryCellUI>().Init(this, item);
            inventoryCells[currentCell].GetComponent<InventoryCellUI>().Init(this, item);
            currentCell++;
            if (currentCell >= inventoryMax + 1) break; // Prevents overflow
        }

        Debug.Log($"Inventory loaded with {inventory.Count} items.");
    }

    public void SelectItem(InventoryCellUI cell)
    {   
        ItemData item = cell.item;
        selectedCell = cell;

        if (item != null) 
        {
            Debug.Log($"Selected item: {item}");

            selectedItem = item;
            itemName.text = item.itemName;
            itemDescription.text = item.description;
            itemImage.sprite = item.icon;

            useButton.interactable = selectedItem.canUse && selectedItem.GetType() == typeof(Consumible);
            equipButton.interactable = selectedItem.canUse && selectedItem.GetType() == typeof(Gear);
        }
        else
        {
            DeselectItem();
        }
    }

    public void DeselectItem()
    {
        selectedItem = null;
        itemName.text = string.Empty;
        itemDescription.text = string.Empty;
        itemImage.sprite = null;
    }

    public void AddItemToInventory(ItemData item)
    {
        // Array.Exists<GameObject>(inventoryCells, itemInCell => itemInCell.GetComponent<InventoryCellUI>().item.itemName == item.itemName)
        bool itemAlreadyInInventory = inventory.Any(i => i.itemName == item.itemName && i.GetType() == item.GetType());
        if (itemAlreadyInInventory && item.isStackable)
        {
            Debug.Log($"Item already in inventory: {item}");
            Debug.Log($"New amount: {item.currentStackSize}");
            ItemData itemInCell = inventoryCells[inventory.IndexOf(item)].GetComponent<InventoryCellUI>().item;
            itemInCell.currentStackSize += 1;
            inventoryCells[inventory.IndexOf(item)].GetComponent<InventoryCellUI>().StackUpdate(itemInCell.currentStackSize);
        }
        else
        {
            inventory.Add(item);
        }
            
        LoadInventoryData();
    }

    public void UseItem()
    {
        if (selectedItem == null) return;
        if(selectedItem.canUse)
        {
            Debug.Log($"Using item: {selectedItem}");
            ConsumeItem();
        }
        else
        {
            Debug.LogWarning("Item cannot be used.");
        }
    }

    private void ConsumeItem()
    {
        if (selectedItem == null) return;
        if (selectedItem.canUse)
        {
            Debug.Log($"Consuming item: {selectedItem}");
            // Define effect of consumable item here
            Consumible consumibleItem = (Consumible) selectedItem;
            consumibleItem.onUseEvent.Invoke();            
            // Decrease stack size
            int newStackSize = consumibleItem.currentStackSize -= 1;
            if (newStackSize <= 0)
            {
                RemoveItemFromInventory();
                return;
            }
            selectedCell.StackUpdate(newStackSize);
        }
        else
        {
            Debug.LogWarning("Item cannot be consumed.");
        }
    }

    public void RemoveItemFromInventory()
    {
        Debug.Log($"Removing item: {selectedItem} from inventory.");
        inventory.Remove(selectedItem);
        selectedCell.GetComponent<InventoryCellUI>().EmptyCell();
        DeselectItem();
    }
}
