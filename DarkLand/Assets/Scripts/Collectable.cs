using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.VersionControl;
using UnityEngine;

public class Collectable : IInteractableObject
{
    // Start is called before the first frame update
    private InventoryManager inventory;
    void Start()
    {
        inventory = Managers.Inventory;
        interactableTrigger = GetComponent<InteractableTrigger>();
        interactableTrigger.isCollectable = true;
        interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.COLLECT_ITEM);
    }
    public override void Interact(){
        inventory?.AddItem(gameObject.name);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
