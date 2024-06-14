using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogsKeeper : MonoBehaviour
{
    private static readonly Dictionary<string, string> ENTITY_NAME_AND_DIALOG;
    public static readonly Dictionary<GameEvent.InteractWithMessage, string> INTERACTION_LABEL;
    //public  const string TALK_TO_TEXT = "[T] to talk";
    public  const string INTERACT_LABEL_TEXT = "[E] Interact";
    static DialogsKeeper(){
        ENTITY_NAME_AND_DIALOG = new Dictionary<string, string>
        {
            {"StartBoard", "I locked you inside, stay safe. At the first floor, next to the bathroom there are some cans... It's better to throw than eat them."},
            {"WeeklyBoard", "Hide the keys and pray."},
            {"ForestBoardFirst", "To the employer:\n DO NOT GO INTO THE FOREST, THOSE CRAETURES ARE HORRIBLE.\nIN CASE YOU DECIDE TO GO...GOOD LUCK AND HIDE INTO THE BIG BUSHES AS THIS BEHIND"},
            {"WeaponBoard", "Somewhere around here I left a gun and some boxes.\nCollect them.\n\nSincerely,\nDR. Eam"},
            {"VillaBoard", "The gate is locked! Stay Away! \nYou can't escape!\nI left my key where the souls rest, I hope someone has moved it inside..."},
            {"VillaWarningBoard", "I left the key in the villa over there! I thought it was a safe place.\n\n It was not."},
            {"FinalBossAreaBoard", "I heard it, that thing inside. It's eating the doctor, the poor doctor with the radio. I'm gonna take that radio!\n Good luck to me!\n\nThe one who lives!"},
            {"Help", "Press TAB to show/hide inventory and commands"}
        };

        INTERACTION_LABEL = new Dictionary<GameEvent.InteractWithMessage, string>{
            {GameEvent.InteractWithMessage.TALK_TO_NPC, "Read"}, 
            {GameEvent.InteractWithMessage.COLLECT_ITEM, "Collect"},
            {GameEvent.InteractWithMessage.INTERACT, "Interact"}, 
            {GameEvent.InteractWithMessage.HIDE, "Hide"},
            {GameEvent.InteractWithMessage.UNHIDE, "Get out"},
            {GameEvent.InteractWithMessage.OPEN_DOOR, "Open door"},
            {GameEvent.InteractWithMessage.CLOSE_DOOR, "Close door"},
            {GameEvent.InteractWithMessage.UNLOCK, "Unlock door"},
            {GameEvent.InteractWithMessage.TURN_ON, "Turn on lights"},
            {GameEvent.InteractWithMessage.TURN_OFF, "Turn off lights"},
            {GameEvent.InteractWithMessage.SAVE_GAME, "Save game"},
            {GameEvent.InteractWithMessage.EXIT_DOOR, "Exit"},
            {GameEvent.InteractWithMessage.OPEN_AMMO_BOX, "Open box"},
            {GameEvent.InteractWithMessage.UNLOCK_AMMO_BOX, "Unlock box"},
            {GameEvent.InteractWithMessage.OPEN_GATE, "Open gate"},
            {GameEvent.InteractWithMessage.CLOSE_GATE, "Close gate"},
            {GameEvent.InteractWithMessage.UNLOCK_GATE, "Unlock gate"},
        };

    }

    public static string GetDialogContent(string key){
        return ENTITY_NAME_AND_DIALOG[key];
    }
    public static string GetButtonContent(string key){
        return "continue";
    }

}
