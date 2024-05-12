using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogsKeeper : MonoBehaviour
{
    private static readonly Dictionary<string, string> ENTITY_NAME_AND_DIALOG;
    //public  const string TALK_TO_TEXT = "[T] to talk";
    public  const string INTERACT_LABEL_TEXT = "[E] Interact";
    static DialogsKeeper(){
        ENTITY_NAME_AND_DIALOG = new Dictionary<string, string>
        {
            {"NPC_1", "first text"},
            {"NPC_2", "second text"}
        };
    }

    public static string GetDialogContent(string key){
        return ENTITY_NAME_AND_DIALOG[key];
    }
    public static string GetButtonContent(string key){
        return "continue";
    }

}
