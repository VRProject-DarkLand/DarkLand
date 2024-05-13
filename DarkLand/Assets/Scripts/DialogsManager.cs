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
        Messenger<string, string>.AddListener(GameEvent.INTERACTION_ENABLED_MESSAGE, ActivateTalkToText);
        Messenger<string, string>.AddListener(GameEvent.INTERACTION_DISABLED_MESSAGE, DeactivateTalkToText);
        //Messenger<string, string>.AddListener(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), DeactivateTalkToText);
        Messenger<string>.AddListener(GameEvent.OPEN_DIALOG, CreateDialog);
        Messenger<string>.AddListener(GameEvent.CLOSE_DIALOG, CloseDialog);
    }
    public void OnDestroy(){
        Messenger<string, string>.RemoveListener(GameEvent.INTERACTION_ENABLED_MESSAGE, ActivateTalkToText);
        Messenger<string, string>.RemoveListener(GameEvent.INTERACTION_DISABLED_MESSAGE, DeactivateTalkToText);
        //Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_ENTERED_NPC_RANGE.ToString(), ActivateTalkToText);
        //Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), DeactivateTalkToText);
        Messenger<string>.RemoveListener(GameEvent.CLOSE_DIALOG, CloseDialog);
        Messenger<string>.RemoveListener(GameEvent.OPEN_DIALOG, CreateDialog);
    }
    void ActivateTalkToText(string entityName, string _interactionMessage){
        if(talkToTextVisible){
            DeactivateTalkToText("", "");
        }
        _interactionMessage = DialogsKeeper.INTERACT_LABEL_TEXT;
        talkToEntityName = entityName;
        talkToText = Instantiate(interactionPopupPrefab, canvas.transform) as GameObject;
        talkToText.GetComponentInChildren<Text>().text = _interactionMessage;
        //Debug.Log("Created talk to TEXT with name " + entityName);
        RectTransform uiElementRectTransform = talkToText.GetComponent<RectTransform>();
        uiElementRectTransform.anchoredPosition = new Vector2(0, 0);
        uiElementRectTransform.localScale = Vector3.one;
        talkToTextVisible = true;
    }
    void DeactivateTalkToText(string entityName, string _interactionMessage){
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
