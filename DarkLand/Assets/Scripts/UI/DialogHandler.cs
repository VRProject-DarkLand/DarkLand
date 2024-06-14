using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI DialogContent;
    [SerializeField] private TextMeshProUGUI continueButtonText;

    private string continueButtonString;
    private string dialogContentString;
    [SerializeField] private Image DialogScreenBackground;

    private string currentText = "";


    //Takes the dialog content string and makes a new character appear in the Text Component every 0.05 seconds 
    IEnumerator RevealText(){
        DialogScreenBackground.enabled = true;
        for (int i = 0; i <= dialogContentString.Length; i++){
            currentText = dialogContentString.Substring(0, i);
            DialogContent.text = currentText;
            yield return new WaitForSeconds(0.05f);
        } 
        DialogScreenBackground.enabled = false;
    }
    //creates a dialog with the passed npc
    //both content and button text for the dialog are taken from outside
    public void OpenDialog(string npcName){
        dialogContentString = DialogsKeeper.GetDialogContent(npcName);
        continueButtonString = DialogsKeeper.GetButtonContent(npcName);
        continueButtonText.text = continueButtonString;
        gameObject.SetActive(true);
        StartCoroutine(RevealText());
    }
    public void DestroyDialog(){   
        currentText = "";
        DialogContent.text = "";
        enabled = false;
        gameObject.SetActive(false);
        Messenger<string>.Broadcast(GameEvent.CLOSE_DIALOG, gameObject.name);
        Destroy(gameObject);
    }
}
