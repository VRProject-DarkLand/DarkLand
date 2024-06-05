using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEntity : IInteractableObject{
    private bool canInteract = false;
    private bool openedDialog = false;
    void Start(){
        interactableTrigger = GetComponent<InteractableTrigger>();
        interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.TALK_TO_NPC);
    }
    public override void Interact(){
        if(!GameEvent.isInDialog)
            Messenger<string>.Broadcast(GameEvent.OPEN_DIALOG, gameObject.name);
        
    }
}
