using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUp : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera wakeUpCamera;
    [SerializeField] private GameObject player;

    void Awake(){
        Messenger.AddListener(GameEvent.SAVE_FINISHED, DestroyCameraAfterInitialSave);
    }
    void OnDestroy(){
    Messenger.RemoveListener(GameEvent.SAVE_FINISHED, DestroyCameraAfterInitialSave);
    }
    public void wakeUp()
    {
        if (!Settings.LoadedFromSave)
        {
            //enable again player movement and mouse roation
            playerCamera.enabled = true;
            wakeUpCamera.enabled = false;
            player.GetComponent<FPSInput>().enabled = true;
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<MouseLook>().enabled = true;
            //notify to create help dialog at the beginning of a new game
            Messenger<string>.Broadcast(GameEvent.OPEN_DIALOG_WITHOUT_TALK_TO_TEXT, "Help");
            GetComponent<Camera>().enabled = false;
            //make a preliminary save of the game such that the created profile
            //is not lost even if the user quits after creating a new game
            StartCoroutine(Managers.Persistence.SaveGame());
        }
    }
    //called as animation event of the camera when new game is started
    //temporarily disables move and camera control from the player
    public void StartWakeUpAnimation(){
        playerCamera.enabled = false;
        wakeUpCamera.enabled = true;
        player.GetComponent<FPSInput>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<MouseLook>().enabled = false;
    }
    void DestroyCameraAfterInitialSave(){
        Destroy(gameObject);
    }
}
