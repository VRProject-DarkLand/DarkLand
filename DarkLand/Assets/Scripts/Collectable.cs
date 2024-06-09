using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.VersionControl;
using UnityEngine;

public class Collectable : IInteractableObject, IDataPersistenceSave{
    // Start is called before the first frame update
    private InventoryManager inventory;
    [SerializeField] private int maxUsages = 1;
    [SerializeField] public bool Collected = false;
    [SerializeField] private string  collectedName = "" ;
    void Start()
    {
        inventory = Managers.Inventory;
        interactableTrigger = GetComponent<InteractableTrigger>();
        interactableTrigger.isCollectable = true;
        interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.COLLECT_ITEM);
        if(interactionSound == null){
            interactionSound = ResourceLoader.GetSound("pickUpSound");
        }
    }
    public override void Interact(){
        bool InsertResult = inventory ? inventory.AddItem(gameObject, maxUsages) : false;
        if(InsertResult)
        {
                        
            Managers.AudioManager.PlaySound(interactionSound);
            Destroy(gameObject);
        }   
    }
    public int GetMaxUsages(){
        return maxUsages;
    }
    public string InventoryName{get =>  collectedName != ""? collectedName:gameObject.name;}
    public void SaveData(){
        if(!Collected && !GameEvent.exitingCurrentScene){
            Settings.gameData.collectableItemsNames.Add(transform.name);
            if(collectedName == "")
                Settings.gameData.collectableItemsPrefabs.Add(transform.name);
            else
                Settings.gameData.collectableItemsPrefabs.Add(collectedName);
            Settings.gameData.collectableItemsPosition.Add(transform.position);
            Settings.gameData.collectableItemsRotation.Add(transform.localEulerAngles);
            Settings.gameData.collectableItemsScale.Add(transform.localScale);
        }   
    }
}
