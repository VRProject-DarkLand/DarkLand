using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsLeverInteractable : IInteractableObject, IDataPersistenceSave{
    [SerializeField] LightsManager manager;
    // Start is called before the first frame update
    void Start()
    {
        interactableTrigger = GetComponent<InteractableTrigger>();
        if(manager.on)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.TURN_OFF);
        else 
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.TURN_ON);
    }

    public override void Interact(){
        gameObject.GetComponent<Animator>().SetBool("On", !manager.on);
        Messenger<bool>.Broadcast(GameEvent.OPERATE_ON_LIGHTS, !manager.on);
        if(manager.on)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.TURN_OFF);
        else 
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.TURN_ON);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData(){
        Settings.gameData.allLightsStatus = manager.on;
    }
}
