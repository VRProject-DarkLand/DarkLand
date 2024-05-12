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
    public  bool isInDialog;


    // // Update is called once per frame
    // void Update(){
    //     if(Input.GetKey(KeyCode.T) && talkToTextVisible){
    //         CreateDialog();
    //     }  
    // }
    void Start(){
        
    }
    public void Awake(){
        Messenger<string, string>.AddListener(GameEvent.InteractionEnabledMessage, ActivateTalkToText);
        Messenger<string, string>.AddListener(GameEvent.InteractionDisabledMessage, DeactivateTalkToText);
        //Messenger<string, string>.AddListener(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), DeactivateTalkToText);
        Messenger<string>.AddListener(GameEvent.OpenDialog, CreateDialog);
        Messenger<string>.AddListener(GameEvent.CloseDialog, CloseDialog);
    }
    public void OnDestroy(){
        Messenger<string, string>.RemoveListener(GameEvent.InteractionEnabledMessage, ActivateTalkToText);
        Messenger<string, string>.RemoveListener(GameEvent.InteractionDisabledMessage, DeactivateTalkToText);
        //Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_ENTERED_NPC_RANGE.ToString(), ActivateTalkToText);
        //Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), DeactivateTalkToText);
        Messenger<string>.RemoveListener(GameEvent.CloseDialog, CloseDialog);
        Messenger<string>.RemoveListener(GameEvent.OpenDialog, CreateDialog);
    }
    void ActivateTalkToText(string entityName, string _interactionMessage){
        if(talkToTextVisible){
            DeactivateTalkToText("", "");
        }
        Debug.Log("Creating talkToText FOR " + entityName +" "+ Time.frameCount);
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
       Debug.Log("Deleting talk to text");
        if(talkToTextVisible){
            //talkToEntityName = null;
            Destroy(talkToText);
            //Debug.Log("Destroyed talk to TEXT");
        }
        talkToTextVisible = false;
    }

    public void CreateDialog(string entityName){
        GameEvent.isInDialog = true;
        Debug.Log("Creating dialog");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject currentDialog = Instantiate(dialogPrefab, canvas.transform);
        DialogHandler dialogHandler = currentDialog.GetComponent<DialogHandler>();
        if(dialogHandler!= null){
            dialogHandler.OpenDialog(talkToEntityName);
            //destroty talk-to message (I expect only one talk-to to be active at a given time)
            //DeactivateTalkToText("", "");
            talkToText.SetActive(false);
        }
    }

    public void CloseDialog(string entityName){
        GameEvent.isInDialog = false;
        Debug.Log("Closing dialog");
        //reactivate talkToDialog  
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;      
        talkToTextVisible = true;
        //ActivateTalkToText(talkToEntityName, _interactionMessage);
        talkToText.SetActive(true);
    }
}
