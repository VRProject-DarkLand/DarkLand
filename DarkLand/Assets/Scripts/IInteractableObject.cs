using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableTrigger))]

public abstract class IInteractableObject: MonoBehaviour{
    protected InteractableTrigger interactableTrigger;
    public abstract void Interact();
}
