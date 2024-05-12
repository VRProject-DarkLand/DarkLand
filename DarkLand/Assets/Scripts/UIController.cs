// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UI;

// public class UIController : MonoBehaviour
// {
//     //name of entity that is close to the player
//     private string talkToEntityName;
//     //prefabs for dialog and talk to label
//     [SerializeField] private GameObject interactionPopupPrefab;
//     [SerializeField] private GameObject dialogPrefab;

//     //talk to object
//     private GameObject talkToText = null;

//     private GameObject openDoorText = null;
//     private bool talkToTextVisible = false;
//     private string _interactionMessage;
//     public  bool isInDialog;

//     public void Update(){
//         if(Input.GetKey(KeyCode.T) && talkToTextVisible){
//             CreateDialog();
//         }
//         isInDialog = GameEvent.isInDialog;
//     }

//     public void CreateDialog(){
//         GameEvent.isInDialog = true;
//         Debug.Log("Creating dialog");
//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;
//         GameObject currentDialog = Instantiate(dialogPrefab, this.transform);
//         DialogHandler dialogHandler = currentDialog.GetComponent<DialogHandler>();
//         if(dialogHandler!= null){
//             dialogHandler.OpenDialog(talkToEntityName);
//             //destroty talk-to message (I expect only one talk-to to be active at a given time)
//             DeactivateTalkToText("", "");
//         }
//     }

//     public void CloseDialog(){
//         GameEvent.isInDialog = false;
//         Debug.Log("Closing dialog");
//         //reactivate talkToDialog  
//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;      
//         talkToTextVisible = true;
//         ActivateTalkToText(talkToEntityName, _interactionMessage);
//     }

//     public void Awake(){
//         Messenger<string, string>.AddListener(GameEvent.DialogTypes.PLAYER_ENTERED_NPC_RANGE.ToString(), ActivateTalkToText);
//         Messenger<string, string>.AddListener(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), DeactivateTalkToText);
//         Messenger<string, string>.AddListener(GameEvent.DialogTypes.PLAYER_ENTERED_DOOR_RANGE.ToString(), ActivateOpenDoorText);
//         Messenger<string, string>.AddListener(GameEvent.DialogTypes.PLAYER_EXIT_DOOR_RANGE.ToString(), DeactivateOpenDoorText);
//         Messenger.AddListener(GameEvent.CLOSED_DIALOG, CloseDialog);
//     }
//     public void OnDestroy(){
//         Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_ENTERED_NPC_RANGE.ToString(), ActivateTalkToText);
//         Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), DeactivateTalkToText);
//         Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_ENTERED_DOOR_RANGE.ToString(), ActivateOpenDoorText);
//         Messenger<string, string>.RemoveListener(GameEvent.DialogTypes.PLAYER_EXIT_DOOR_RANGE.ToString(), DeactivateOpenDoorText);
//         Messenger.RemoveListener(GameEvent.CLOSED_DIALOG, CloseDialog);
//     }



//     void ActivateTalkToText(string entityName, string _interactionMessage){
//         //Debug.Log("Creating talkToText");
//         _interactionMessage = DialogsKeeper.TALK_TO_TEXT;
//         talkToEntityName = entityName;
//         talkToText = Instantiate(interactionPopupPrefab, this.transform) as GameObject;
//         talkToText.GetComponentInChildren<Text>().text = _interactionMessage;
//         //Debug.Log("Created talk to TEXT with name " + entityName);
//         RectTransform uiElementRectTransform = talkToText.GetComponent<RectTransform>();
//         uiElementRectTransform.anchoredPosition = new Vector2(0, 0);
//         uiElementRectTransform.localScale = Vector3.one;
//         talkToTextVisible = true;
//     }
//     void ActivateOpenDoorText(string entityName, string _interactionMessage){
//         Debug.Log("Creating open door text");
//         //talkToEntityName = entityName;
//         openDoorText = Instantiate(interactionPopupPrefab, this.transform) as GameObject;
//         openDoorText.GetComponentInChildren<Text>().text = DialogsKeeper.TALK_TO_TEXT;
//         RectTransform uiElementRectTransform = openDoorText.GetComponent<RectTransform>();
//         uiElementRectTransform.anchoredPosition = new Vector2(0, 0);
//         uiElementRectTransform.localScale = Vector3.one;
//     }

//     void DeactivateOpenDoorText(string entityName, string _interactionMessage){

//     }

//     void DeactivateTalkToText(string entityName, string _interactionMessage){
//        //Debug.Log("Deleting talk to text");
//         if(talkToTextVisible){
//             //talkToEntityName = null;
//             Destroy(talkToText);
//             //Debug.Log("Destroyed talk to TEXT");
//         }
//         talkToTextVisible = false;
//     }

// }
