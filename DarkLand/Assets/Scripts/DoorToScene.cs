using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToScene : IInteractableObject
{

    [SerializeField] private bool requireKey = false;
    [SerializeField] private string key = "Key";
    [SerializeField] private string scene;

    public override void Interact()
    {
        if (scene == null)
        {return;}
        if(CanInteract()){
            if(requireKey){
                requireKey = false;
                //Managers.Inventory.ConsumeItem(key);
                interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.EXIT_DOOR);
                return;
            }
            GameEvent.exitingCurrentScene = true;
            GameEvent.newScene = scene;
            GameEvent.OpenedSceneDoor = true;
            StartCoroutine(Managers.Persistence.SaveGame());
        }
    }

    public override bool CanInteract()
    {
        if(requireKey){
            if(Managers.Inventory.GetItemCount(key) == 0)
                return false;
        }
        return true;

    }

    // Start is called before the first frame update
    void Start()
    {
         interactableTrigger = GetComponent<InteractableTrigger>();
         interactableTrigger = GetComponent<InteractableTrigger>();
        if(!requireKey)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.EXIT_DOOR);
        else
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.UNLOCK);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
