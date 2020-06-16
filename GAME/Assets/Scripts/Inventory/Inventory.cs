using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // this is the actual data that will store what the player currently has in the inventory.
    private Dictionary<string, int> inventory = new Dictionary<string, int>();


    public void UpdateItem(Item item)
    {
        string itemName = item.name;
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += 1;
        }
        else
        {
            inventory.Add(itemName, 1);
        }
        Debug.Log("Updated the item into the storage");
    }
    public GameObject GetItemByName(Vector3 location, string name)
    {
        // we need to first check if the player has the item
        if(inventory.ContainsKey(name) && inventory[name] > 0)
        {
            // if the player has the item then we are just going to instantiate it. 
            GameObject instantiatedItem = Instantiate(InventoryManager.inventoryManager.GetItemPrefab(name), location, Quaternion.identity);
            // we need to tell the inventory that we just grabbed an item from the inventory
            inventory[name] -= 1;
            return instantiatedItem;
        }
        else
        {
            // this is for debugging purposes. 
            Debug.Log($"The inventory does not have the name {name}");
            return null;
        }
        
    }

}
