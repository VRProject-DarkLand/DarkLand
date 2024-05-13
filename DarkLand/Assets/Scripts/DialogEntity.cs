using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEntity : IInteractableObject{
    private bool canInteract = false;
    private bool openedDialog = false;

    public override void Interact(){
        Messenger<string>.Broadcast(GameEvent.OPEN_DIALOG, gameObject.name);
    }
}
