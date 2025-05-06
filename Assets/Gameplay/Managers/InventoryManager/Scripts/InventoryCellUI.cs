using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCellUI : Selectable, ISubmitHandler
{
    public ItemData item;
    
    private InventoryManagerSingleton inventoryManager;
    private Image cellItemImage;
    private GameObject stackTextObject;

    public void Init(InventoryManagerSingleton manager, ItemData itemData)
    {
        inventoryManager = manager;
        item = itemData;

        if (item.isStackable)
        {
            Consumible consumableItem = (Consumible)item;
            stackTextObject = GameObject.Find("StackText");
            if (stackTextObject == null)
            {
                stackTextObject = new GameObject("StackText", typeof(RectTransform), typeof(TextMeshProUGUI));
                stackTextObject.transform.SetParent(this.transform, false);

                TextMeshProUGUI stackText = stackTextObject.GetComponent<TextMeshProUGUI>();
                stackText.text = consumableItem.currentStackSize.ToString();
                stackText.fontSize = 14;
                stackText.color = Color.red;
                stackText.alignment = TextAlignmentOptions.Center;
                stackText.rectTransform.sizeDelta = new Vector2(17, 14);
                stackText.rectTransform.anchoredPosition = new Vector2(11, -13);
            }
        }

        LoadItemSprite();
    }

    public void EmptyCell() {
        item = null;
        cellItemImage.sprite = null;
        cellItemImage.color = Color.clear;
        TextMeshProUGUI stackableLabel = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (stackableLabel != null)
                stackableLabel.text = string.Empty;
    }

    private void LoadItemSprite()
    {
        cellItemImage = transform.GetChild(0).GetComponent<Image>();
        cellItemImage.sprite = item.icon;
        cellItemImage.color = Color.white;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        InventoryManagerSingleton.Instance.SelectItem(eventData.selectedObject.GetComponent<InventoryCellUI>());

        base.OnSelect(eventData);   
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        // InventoryManagerSingleton.Instance.DeselectItem();

        base.OnDeselect(eventData);
    }

    public void StackUpdate(int newStackSize)
    {
        if (item == null) return;
        if (item.isStackable)
        {
            Consumible consumableItem = (Consumible)item;
            consumableItem.currentStackSize = newStackSize;

            if (newStackSize > consumableItem.maxStackSize)
            {
                newStackSize = consumableItem.maxStackSize;
            }
            consumableItem.currentStackSize = newStackSize;
            stackTextObject.GetComponent<TextMeshProUGUI>().text = consumableItem.currentStackSize.ToString();
        }
    }
}
