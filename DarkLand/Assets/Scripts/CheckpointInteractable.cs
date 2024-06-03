using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointInteractable : IInteractableObject{
    private bool _canSave = false;
    public override void Interact(){
        if(_canSave){
            StartCoroutine(Managers.Persistence.SaveGame());
            _canSave = false;
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
        yield return new WaitForSeconds(10f);
        _canSave = true;
    }
}
