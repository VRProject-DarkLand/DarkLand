using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointInteractable : IInteractableObject{
    private bool _canSave = false;
    public override void Interact(){
        if(_canSave){
            _canSave = false;
            StartCoroutine(Managers.Persistence.SaveGame());
            StartCoroutine(WaitForNextSave());
            //interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.SAVE_GAME);
        }else{

        }
        //Debug.Log("Asked to save");
    }
    void Start(){
        interactableTrigger = GetComponent<InteractableTrigger>();
        interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.SAVE_GAME);
        //Debug.Log("Checkpoint message set");
        StartCoroutine(WaitForNextSave());
    }
    private IEnumerator WaitForNextSave(){
        yield return new WaitForSeconds(1f);
        _canSave = true;
    }
}
