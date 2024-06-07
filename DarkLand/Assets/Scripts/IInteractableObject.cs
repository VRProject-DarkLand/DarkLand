using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableTrigger))]

public abstract class IInteractableObject: MonoBehaviour{
    protected InteractableTrigger interactableTrigger;
    [SerializeField] protected AudioClip interactionSound;
    public abstract void Interact();
    public virtual bool CanInteract(){return true;}
    public virtual void ReactiveInteraction(){}
}
