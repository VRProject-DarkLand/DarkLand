using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUp : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera wakeUpCamera;
    [SerializeField] private GameObject player;


    public void wakeUp()
    {
        if (!Settings.LoadedFromSave)
        {
            playerCamera.enabled = true;
            wakeUpCamera.enabled = false;
            player.GetComponent<FPSInput>().enabled = true;
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<MouseLook>().enabled = true;
            Destroy(gameObject);
            Messenger<string>.Broadcast(GameEvent.OPEN_DIALOG_WITHOUT_TALK_TO_TEXT, "Help");
        }
    }
    public void StartWakeUpAnimation(){
        playerCamera.enabled = false;
        wakeUpCamera.enabled = true;
        player.GetComponent<FPSInput>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<MouseLook>().enabled = false;
    }
}
