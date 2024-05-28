using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryViewer : MonoBehaviour
{
    [SerializeField] private GameObject gridPanel;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private GameObject inventorySlot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Show(){
        foreach(string item in Managers.Inventory.GetItemList()){
            GameObject slot = Instantiate(inventorySlot);
            slot.transform.SetParent(gridPanel.transform, false);
            if(item.EndsWith("Key", StringComparison.OrdinalIgnoreCase)){
                Sprite sprite = Resources.Load<Sprite>("InventoryIcons/"+"key");
                slot.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            }else{
                Sprite sprite = Resources.Load<Sprite>("InventoryIcons/"+item);
                slot.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            }
        }
    }

    public void Clean(){
        foreach (Transform child in gridPanel.transform) {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
