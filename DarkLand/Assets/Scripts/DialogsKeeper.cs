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
            {"NPC_1", "first text"},
            {"NPC_2", "second text"}
        };

        INTERACTION_LABEL = new Dictionary<GameEvent.InteractWithMessage, string>{
            {GameEvent.InteractWithMessage.TALK_TO_NPC, "Talk"}, 
            {GameEvent.InteractWithMessage.COLLECT_ITEM, "Collect"},
            {GameEvent.InteractWithMessage.INTERACT, "Interact"}, 
            {GameEvent.InteractWithMessage.HIDE, "Hide"},
            {GameEvent.InteractWithMessage.UNHIDE, "Get out"},
            {GameEvent.InteractWithMessage.OPEN_DOOR, "Open door"},
            {GameEvent.InteractWithMessage.CLOSE_DOOR, "Close door"},
            {GameEvent.InteractWithMessage.UNLOCK, "Unlock door"},

        };

    }

    public static string GetDialogContent(string key){
        return ENTITY_NAME_AND_DIALOG[key];
    }
    public static string GetButtonContent(string key){
        return "continue";
    }

}
