using UnityEngine;
using UnityEditor;

public class DevManager : MonoBehaviour
{
    public ItemData itemToAdd;

    #region  UNITY_EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(DevManager))]
            public class DevManagerEditor : Editor
            {
                public override void OnInspectorGUI()
                {
                    base.OnInspectorGUI();

                    var devManager = (DevManager)target;

                    if (GUILayout.Button("Add Item to Inventory"))
                    {
                        devManager.itemToAdd = (ItemData)EditorGUILayout.ObjectField("Item to Add", devManager.itemToAdd, typeof(ItemData), false);
                        InventoryManagerSingleton.Instance.AddItemToInventory(devManager.itemToAdd);
                    }
                }
            }
        #endif
    #endregion
}