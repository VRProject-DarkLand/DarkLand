using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Show(){
        var items = Managers.Inventory.GetItems();
        foreach(InventoryItem item in items) {
            GameObject slot = Instantiate(inventorySlot);
            slot.transform.SetParent(gridPanel.transform, false);
            slot.transform.GetChild(0).GetComponent<Image>().sprite = ResourceLoader.GetImage(item.Name);
            AddEvent(slot, EventTriggerType.PointerClick, e => SelectItem(item) );
        }
        if(items.Count > 0)
            SelectItem(items[0]);

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

    void SelectItem(InventoryItem item){
        selectedItem.enabled = true;
        selectedItem.sprite = ResourceLoader.GetImage(item.Name);
        selectedName.text = item.Name+ " x"+item.Count;
        this.description.text = ResourceLoader.GetDescription(item.Name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
