using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class LightsLeverInteractable : IInteractableObject, IDataPersistenceSave{
    [SerializeField] LightsManager manager;
    void Awake(){
        Messenger.AddListener(GameEvent.ALL_MANAGERS_LOADED, ChangeAnimation);
    }
    void OnDestroy(){
        Messenger.RemoveListener(GameEvent.ALL_MANAGERS_LOADED, ChangeAnimation);
    }
    private void ChangeAnimation(){
        if(!manager.on)
            gameObject.GetComponent<Animator>().SetBool("On", false);
    }
    void Start()
    {
        interactableTrigger = GetComponent<InteractableTrigger>();
        if(manager.on)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.TURN_OFF);
        else
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.TURN_ON);
    }

    /// <summary>
    /// Animate the lever and send a message to turn off or on the lights, depending on the contrary of the internal state 
    /// </summary>
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
