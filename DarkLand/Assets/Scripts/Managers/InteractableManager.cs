using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour{

    public static Tuple<InteractableTrigger, float, bool> selectedInteractable = new Tuple<InteractableTrigger, float, bool>(null, 0, true);
    
    private static bool interactableChanged = false;
    //Pass a new interactable object with its looking score
    //and update the interactable if the new arrived has a higher looking score w.r.t. the current selected
    public static void SetInteractableGameObject(Tuple<InteractableTrigger, float, bool> interactable){
        if(interactable.Item1 == selectedInteractable.Item1 ){
            selectedInteractable = interactable;
            return; 
        }
        if(selectedInteractable.Item1 != null && interactable.Item1 != null)
            Debug.Log(selectedInteractable.Item1.gameObject.name + " " + selectedInteractable.Item2 +" vs "+ interactable.Item1.gameObject.name + " " + interactable.Item2  );
        if(interactable.Item2 >= selectedInteractable.Item2 || selectedInteractable.Item1 == null ){
            selectedInteractable = interactable;
            interactableChanged = true;
        }
    }
    //update the selected interactable by considering that interactable can no longer be selected
    public static void DeselectInteractableGameObject(InteractableTrigger interactable){
        if(selectedInteractable.Item1 != null){
            if(interactable == selectedInteractable.Item1){
                    interactableChanged = true;
                    Messenger.Broadcast(GameEvent.INTERACTION_DISABLED_MESSAGE, MessengerMode.DONT_REQUIRE_LISTENER);
                    selectedInteractable = new Tuple<InteractableTrigger, float, bool>(null, 0,true);
            }
        }
    }
    //call interact on the current selected item
    public static void InteractWithSelectedItem(bool collectable){
        if(selectedInteractable.Item1 != null){
            if( collectable == selectedInteractable.Item1.isCollectable && selectedInteractable.Item3)
                selectedInteractable.Item1.SendMessage("Interact");
        }
    }
    //if the currentInteractable has changed during the frame that is being processed,
    //change the interaction message that is shown to the player
    //this is done in the late update to ensure that the setInteractable is called by
    //all the interactables s.t. the player is inside their trigger 
    void LateUpdate(){
        if(selectedInteractable.Item1 != null && interactableChanged){
            //Debug.Log("Late update");
            Messenger<string, GameEvent.InteractWithMessage, bool>.Broadcast(GameEvent.INTERACTION_ENABLED_MESSAGE, selectedInteractable.Item1.name, selectedInteractable.Item1.InteractionMessage, selectedInteractable.Item3);
        }
        interactableChanged = false;
    }
}