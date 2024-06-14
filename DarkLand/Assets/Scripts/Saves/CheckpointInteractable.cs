using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointInteractable : IInteractableObject{
    public override void Interact(){
        if(Settings.canSave){
            Settings.canSave = false;
            StartCoroutine(Managers.Persistence.SaveGame());
            StartCoroutine(WaitForNextSave());
            //interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.SAVE_GAME);
        }else{

        }
        //Debug.Log("Asked to save");
    }
    public override bool CanInteract(){
        return Settings.canSave;
    }
    void Start(){
        interactableTrigger = GetComponent<InteractableTrigger>();
        interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.SAVE_GAME);
        //Debug.Log("Checkpoint message set");
        StartCoroutine(WaitForNextSave());
    }
    private IEnumerator WaitForNextSave(){
        yield return new WaitForSeconds(3f);
        Settings.canSave = true;
    }
}
