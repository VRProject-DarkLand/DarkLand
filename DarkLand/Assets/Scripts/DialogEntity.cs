using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableTrigger))]
public class DialogEntity : MonoBehaviour, IInteractableObject{
    private bool canInteract = false;
    private bool openedDialog = false;

    public void Interact(){
        Messenger<string>.Broadcast(GameEvent.OpenDialog, gameObject.name);
    }
}
