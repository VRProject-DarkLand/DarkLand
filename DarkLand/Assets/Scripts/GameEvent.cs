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
    public const string OPERATE_ON_LIGHTS = "OPERATE_LIGHTS";
    public const string PREDICT_TRAJECTORY = "PREDICT_TRAJECTORY";
    public const string CANCEL_TRAJECTORY = "CANCEL_TRAJECTORY";
    public const string IS_HIDING = "HIDING";
    public const string CHANGED_HEALTH = "HEALTH_CHANGED";
    public const string PLAYER_DEAD = "PLAYER_DEAD";

    public const string PAUSED = "PAUSED";
    public const string USED_USABLE = "USED_USABLE";
    public const string CHANGED_SELECTABLE = "CHANGED_SELECTABLE";
    public const string SHOW_INVENTORY = "SHOW_INVENTORY";
    public const string USABLE_ADDED = "USABLE_ADDED";
    public const string LOADING_VALUE = "LOADING_VALUE";
    public const string CHANGING_SCENE = "CHANGING_SCENE";
    public const string ALL_MANAGERS_LOADED = "ALL_MANAGERS_LOADED";
    public static bool isInDialog = false;
    public static bool isHiding = false ;
    public static bool isInventoryOpen = false ;
    public static bool isUsingGun = false;
    public static HashSet<int> chasingSet = new();

    public enum InteractWithMessage{
        TALK_TO_NPC,
        HIDE,
        UNHIDE,
        OPEN_DOOR,
        EXIT_DOOR,
        CLOSE_DOOR,
        COLLECT_ITEM,
        INTERACT,
        UNLOCK,
        TURN_ON,
        TURN_OFF,
        SAVE_GAME,
    };

}
