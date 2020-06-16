using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // because this gameobject only needs to exist, I made it such that the player can access it without grabbing an instance of it. 
    public static InventoryManager inventoryManager;

    // this list is the list of objects that are available within the game.
    [SerializeField]
    private List<GameObject> itemPrefabs = new List<GameObject>();

    // this dictionary is created so that the player can access the items as quickly as possible. It has a O(1) time complexity so. 
    private Dictionary<string, GameObject> nameOfItem = new Dictionary<string, GameObject>();


    // this is used to grab each Item instance within each prefab. 
    private Item itemInstance;

    private void Awake()
    {
        int index = 0;
        inventoryManager = gameObject.GetComponent<InventoryManager>();
        foreach(GameObject prefab in itemPrefabs)
        {
            itemInstance = prefab.GetComponent<Item>();
            if (itemInstance != null)
            {
                nameOfItem.Add(itemInstance.name, prefab);
            }
            else
            {
                // this part right here is to make debugging easier later on. 
                Debug.Log($"The item at index {index} does not have an Item script");
            }
            index += 1;
        }
    }

    public GameObject GetItemPrefab(string itemName)
    {
        // if the item that we are trying to access exists 
        if(nameOfItem.ContainsKey(itemName))
        {
            return nameOfItem[itemName];
        }
        // if the item doens't exist then we ofc return that it doesn't exist :)
        return null;
    }
}