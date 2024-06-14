using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UsableFlashLight : IUsableObject{
    
    private Light flashLight;
    void Start(){
        flashLight = GetComponentInChildren<Light>();
    }
    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.6f);
        gameObject.transform.localEulerAngles = new Vector3(95f,0f,0f);
        // if(flashLight.enabled)
        //     Managers.AudioManager.PlaySound(useSound);
        //gameObject.transform.position = 
        //set torch visible
    }
    
    /// <summary>
    ///Enable or disable the torchlight if not in dialog. If isHiding is true, then it disables the torchlight 
    /// </summary>
    public override void Use(){
        if(!GameEvent.isInDialog){
            Debug.Log("Changing light status");
            Managers.AudioManager.PlaySound(useSound);
            flashLight.enabled = !gameObject.GetComponentInChildren<Light>().enabled;
        } 
        if(GameEvent.isHiding)
            flashLight.enabled = false;
    }
}
