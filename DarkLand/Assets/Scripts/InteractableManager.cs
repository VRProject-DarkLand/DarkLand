using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour{

    public static Tuple<InteractableTrigger, float> selectedInteractable = new Tuple<InteractableTrigger, float>(null, 0);
    
    private static bool interactableChanged = false;
    public static void SetInteractableGameObject(Tuple<InteractableTrigger, float> interactable, bool isCollectable){
        if(!isCollectable){
            if(interactable.Item1 != selectedInteractable.Item1){
                if(interactable.Item2 > selectedInteractable.Item2 || selectedInteractable.Item1 == null){
                    selectedInteractable = interactable;
                    interactableChanged = true;
                }
            }
        }
    }

    public static void DeselectInteractableGameObject(InteractableTrigger interactable, bool isCollectable){
        if(selectedInteractable.Item1 != null){
            if(!isCollectable){
                if(interactable == selectedInteractable.Item1){
                    interactableChanged = true;
                    Debug.Log("Sending delete message");
                    Messenger<string, string>.Broadcast(GameEvent.InteractionDisabledMessage, selectedInteractable.Item1.name, selectedInteractable.Item1.InteractionMessage.ToString());
                    selectedInteractable = new Tuple<InteractableTrigger, float>(null, 0);
                }
            }
        }
    }
    public static void InteractWithSelectedItem(){
        if(selectedInteractable.Item1 != null){
            selectedInteractable.Item1.SendMessage("Interact");
        }
    }
    void LateUpdate(){
        if(selectedInteractable.Item1 != null && interactableChanged){
            //Debug.Log("Late update");
            Messenger<string, string>.Broadcast(GameEvent.InteractionEnabledMessage, selectedInteractable.Item1.name, selectedInteractable.Item1.InteractionMessage.ToString());
        }
        interactableChanged = false;
    }
}
