using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryViewer : MonoBehaviour
{
    [SerializeField] private GameObject gridPanel;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private GameObject inventorySlot;
    [SerializeField] private Image selectedItem;
    [SerializeField] private TextMeshProUGUI selectedName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject commands;
    [SerializeField] private GameObject next;
    [SerializeField] private GameObject previous;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Show(){
        PanelShow(true);
        var items = Managers.Inventory.GetItemsKey();
        
        foreach(string item in items) {
            InventoryItem invItem = Managers.Inventory.GetItem(item);
            if(invItem == null) continue;
            GameObject slot = Instantiate(inventorySlot);
            slot.transform.SetParent(gridPanel.transform, false);
            slot.transform.GetChild(0).GetComponent<Image>().sprite = ResourceLoader.GetImage(invItem.InventoryClassKey);
            AddEvent(slot, EventTriggerType.PointerClick, e => SelectItem(item, invItem) );
        }
        if(items.Count > 0){
            SelectItem(items[0], Managers.Inventory.GetItem(items[0]));
            descriptionPanel.SetActive(true);
        }else{
            descriptionPanel.SetActive(false);
        }
    }

    public void PanelShow(bool showInventory ){
        inventory.SetActive(showInventory);
        commands.SetActive(!showInventory);
    }

    public void OnCommandsPage(){
        PanelShow(false);
    }
    public void OnInventoryPage(){
        PanelShow(true);
    }
    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (!trigger) { Debug.LogWarning("No EventTrigger component found!"); return; }
        var eventTrigger = new EventTrigger.Entry { eventID = type };
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void Clean(){
        DeselectItem();
        foreach (Transform child in gridPanel.transform) {
            Destroy(child.gameObject);
        }
    }

     void DeselectItem(){
        selectedItem.enabled = false;
        selectedName.text = "";
        description.text = "";
    }

    void SelectItem(string name, InventoryItem item){
        selectedItem.enabled = true;
        selectedItem.sprite = ResourceLoader.GetImage(item.InventoryClassKey);
        selectedName.text = name + " x"+item.Count;
        this.description.text = ResourceLoader.GetDescription(name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
