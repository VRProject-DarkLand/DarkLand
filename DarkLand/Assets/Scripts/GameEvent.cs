using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;



public class GameEvent : MonoBehaviour{
//     public const string PLAYER_ENTERED_NPC_RANGE = "PLAYER_ENTERED_NPC_RANGE";
//     public const string PLAYER_EXIT_NPC_RANGE = "PLAYER_EXIT_NPC_RANGE";
    public const string CLOSE_DIALOG = "CLOSED_DIALOG";
    public const string INTERACTION_ENABLED_MESSAGE = "CAN_INTERACT";
    public const string INTERACTION_DISABLED_MESSAGE = "CANNOT_INTERACT";
    public const string OPEN_DIALOG = "OPEN_DIALOG";
    public const string IS_HIDING = "HIDING";
    public static bool isInDialog = false;
    public static bool isHiding = false;

    public enum InteractWithMessage{
        TALK_TO_NPC,
        OPEN_DOOR,
    };

    public static Tuple<GameObject, float> selectedInteractable = new Tuple<GameObject, float>(null, 0);
    public static void SetInteractableGameObject(Tuple<GameObject, float> interactable, bool isCollectable){
        if(!isCollectable){
            if(interactable.Item2 > selectedInteractable.Item2 || selectedInteractable.Item1 == null){
                selectedInteractable = interactable;
            }
        }
    }
    public static void DeselectInteractableGameObject(GameObject interactable, bool isCollectable){
        if(!isCollectable){
            if(interactable == selectedInteractable.Item1){
                selectedInteractable = new Tuple<GameObject, float>(null, 0);
            }
        }
    }
}
