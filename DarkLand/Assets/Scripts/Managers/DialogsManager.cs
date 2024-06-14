using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private Color blockedInteractionColor = new Color32(170,0,0,255);
    [SerializeField] private Color defaultInteractionColor = Color.black;
    //talk to object
    private GameObject talkToText = null;
    private bool talkToTextVisible = false;
    private string _interactionMessage;

    void Awake(){
        Messenger<string, GameEvent.InteractWithMessage, bool>.AddListener(GameEvent.INTERACTION_ENABLED_MESSAGE, ActivateTalkToText);
        Messenger.AddListener(GameEvent.INTERACTION_DISABLED_MESSAGE, DeactivateTalkToText);
        Messenger<string>.AddListener(GameEvent.OPEN_DIALOG, CreateDialog);
        Messenger<string>.AddListener(GameEvent.CLOSE_DIALOG, CloseDialog);
        Messenger<string>.AddListener(GameEvent.OPEN_DIALOG_WITHOUT_TALK_TO_TEXT, CreateInstantDialogDialog);
    }


    public void OnDestroy(){
        Messenger<string, GameEvent.InteractWithMessage, bool>.RemoveListener(GameEvent.INTERACTION_ENABLED_MESSAGE, ActivateTalkToText);
        Messenger.RemoveListener(GameEvent.INTERACTION_DISABLED_MESSAGE, DeactivateTalkToText);
        Messenger<string>.RemoveListener(GameEvent.CLOSE_DIALOG, CloseDialog);
        Messenger<string>.RemoveListener(GameEvent.OPEN_DIALOG, CreateDialog);
        Messenger<string>.RemoveListener(GameEvent.OPEN_DIALOG_WITHOUT_TALK_TO_TEXT, CreateInstantDialogDialog);
    }
    //create interaction text with the selected interactable item
    void ActivateTalkToText(string entityName,  GameEvent.InteractWithMessage interactionType, bool possibleAction){
        if(talkToTextVisible){
            DeactivateTalkToText();
        }
        string command = "E";
        string _interactionMessage ="";
        if(interactionType == GameEvent.InteractWithMessage.COLLECT_ITEM){
            command = "C";
            _interactionMessage +="["+command+"]  "+DialogsKeeper.INTERACTION_LABEL[interactionType]+" "+entityName;
        }else
            _interactionMessage = "["+command+"]  "+DialogsKeeper.INTERACTION_LABEL[interactionType];
        
        talkToEntityName = entityName;
        talkToText = Instantiate(interactionPopupPrefab, canvas.transform) as GameObject;
        if(possibleAction){
            talkToText.GetComponentInChildren<Text>().color = defaultInteractionColor;
        }else{
            talkToText.GetComponentInChildren<Text>().color = blockedInteractionColor;
        }
        talkToText.GetComponentInChildren<Text>().text = _interactionMessage;
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
    //create dialog with the entity
    public void CreateDialog(string entityName){
        GameEvent.isInDialog = true;
        Managers.PointerManager.UnlockCursor();
        GameObject currentDialog = Instantiate(dialogPrefab, canvas.transform);
        DialogHandler dialogHandler = currentDialog.GetComponent<DialogHandler>();
        if(dialogHandler!= null){
            dialogHandler.OpenDialog(talkToEntityName);
            talkToText.SetActive(false);
        }
    }
    //create dialog without setting the entity to which the player will talk
    public void CreateInstantDialogDialog(string entityName){
        GameEvent.isInDialog = true;
        Managers.PointerManager.UnlockCursor();
        GameObject currentDialog = Instantiate(dialogPrefab, canvas.transform);
        DialogHandler dialogHandler = currentDialog.GetComponent<DialogHandler>();
        if(dialogHandler!= null){
            dialogHandler.OpenDialog(entityName);
        }
    }
    public void CloseDialog(string entityName){
        GameEvent.isInDialog = false;
        //reactivate talkToDialog  
        Managers.PointerManager.LockCursor();
        if(talkToText != null){
            talkToTextVisible = true;
            talkToText.SetActive(true);
        }
    }
}
