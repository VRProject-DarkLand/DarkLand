using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogsManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    //name of entity that is close to the player
    private string talkToEntityName;
    //prefabs for dialog and talk to label
    [SerializeField] private GameObject interactionPopupPrefab;
    [SerializeField] private GameObject dialogPrefab;

    //talk to object
    private GameObject talkToText = null;
    private bool talkToTextVisible = false;
    private string _interactionMessage;

    public void Awake(){
        Messenger<string, GameEvent.InteractWithMessage>.AddListener(GameEvent.INTERACTION_ENABLED_MESSAGE, ActivateTalkToText);
        Messenger.AddListener(GameEvent.INTERACTION_DISABLED_MESSAGE, DeactivateTalkToText);
        //Messenger<string, string>.AddListener(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), DeactivateTalkToText);
        Messenger<string>.AddListener(GameEvent.OPEN_DIALOG, CreateDialog);
        Messenger<string>.AddListener(GameEvent.CLOSE_DIALOG, CloseDialog);
    }
    public void OnDestroy(){
        Messenger<string, GameEvent.InteractWithMessage>.RemoveListener(GameEvent.INTERACTION_ENABLED_MESSAGE, ActivateTalkToText);
        Messenger.RemoveListener(GameEvent.INTERACTION_DISABLED_MESSAGE, DeactivateTalkToText);
        //Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_ENTERED_NPC_RANGE.ToString(), ActivateTalkToText);
        //Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), DeactivateTalkToText);
        Messenger<string>.RemoveListener(GameEvent.CLOSE_DIALOG, CloseDialog);
        Messenger<string>.RemoveListener(GameEvent.OPEN_DIALOG, CreateDialog);
    }
    void ActivateTalkToText(string entityName,  GameEvent.InteractWithMessage interactionType){
        if(talkToTextVisible){
            DeactivateTalkToText();
        }
        string command = "E";
        string _interactionMessage ="";
        if(interactionType == GameEvent.InteractWithMessage.COLLECT_ITEM){
            command = "C";
            _interactionMessage +="["+command+"] "+DialogsKeeper.INTERACTION_LABEL[interactionType]+" "+entityName;
        }else
            _interactionMessage = "["+command+"] "+DialogsKeeper.INTERACTION_LABEL[interactionType];
        
        talkToEntityName = entityName;
        talkToText = Instantiate(interactionPopupPrefab, canvas.transform) as GameObject;
        talkToText.GetComponentInChildren<Text>().text = _interactionMessage;
        //Debug.Log("Created talk to TEXT with name " + entityName);
        RectTransform uiElementRectTransform = talkToText.GetComponent<RectTransform>();
        uiElementRectTransform.anchoredPosition = new Vector2(0, 0);
        uiElementRectTransform.localScale = Vector3.one;
        talkToTextVisible = true;
    }
    void DeactivateTalkToText(){
        if(talkToTextVisible){
            Destroy(talkToText);
        }
        talkToTextVisible = false;
    }

    public void CreateDialog(string entityName){
        GameEvent.isInDialog = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject currentDialog = Instantiate(dialogPrefab, canvas.transform);
        DialogHandler dialogHandler = currentDialog.GetComponent<DialogHandler>();
        if(dialogHandler!= null){
            dialogHandler.OpenDialog(talkToEntityName);
            talkToText.SetActive(false);
        }
    }

    public void CloseDialog(string entityName){
        GameEvent.isInDialog = false;
        //reactivate talkToDialog  
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;      
        talkToTextVisible = true;
        talkToText.SetActive(true);
    }
}
