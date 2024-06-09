using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour{

    public static Tuple<InteractableTrigger, float, bool> selectedInteractable = new Tuple<InteractableTrigger, float, bool>(null, 0, true);
    
    private static bool interactableChanged = false;
    
    public static void SetInteractableGameObject(Tuple<InteractableTrigger, float, bool> interactable){
        if(interactable.Item1 != selectedInteractable.Item1 || (selectedInteractable.Item3 != interactable.Item3)){
            if(interactable.Item2 >= selectedInteractable.Item2 || selectedInteractable.Item1 == null ){
                selectedInteractable = interactable;
                interactableChanged = true;
            }
        }
    }

    public static void DeselectInteractableGameObject(InteractableTrigger interactable){
        if(selectedInteractable.Item1 != null){
            if(interactable == selectedInteractable.Item1){
                    interactableChanged = true;
                    Messenger.Broadcast(GameEvent.INTERACTION_DISABLED_MESSAGE, MessengerMode.DONT_REQUIRE_LISTENER);
                    selectedInteractable = new Tuple<InteractableTrigger, float, bool>(null, 0,true);
            }
        }
    }
    public static void InteractWithSelectedItem(bool collectable){
        if(selectedInteractable.Item1 != null){
            if( collectable == selectedInteractable.Item1.isCollectable && selectedInteractable.Item3)
                selectedInteractable.Item1.SendMessage("Interact");
        }
    }

    void LateUpdate(){
        if(selectedInteractable.Item1 != null && interactableChanged){
            //Debug.Log("Late update");
            Messenger<string, GameEvent.InteractWithMessage, bool>.Broadcast(GameEvent.INTERACTION_ENABLED_MESSAGE, selectedInteractable.Item1.name, selectedInteractable.Item1.InteractionMessage, selectedInteractable.Item3);
        }
        interactableChanged = false;
    }
}
