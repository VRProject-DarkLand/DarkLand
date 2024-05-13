using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableTrigger))]
public abstract class IInteractableObject: MonoBehaviour{
    public abstract void Interact();
}
