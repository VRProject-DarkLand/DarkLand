using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableFlashLight : IUsableObject{
    

    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.6f);
        gameObject.transform.localEulerAngles = new Vector3(95f,0f,0f);
        //gameObject.transform.position = 
        //set torch visible
    }

    public override void Use(){
        if(!GameEvent.isInDialog){
            Debug.Log("Changing light status");
            Managers.AudioManager.PlaySound(useSound);
            gameObject.GetComponentInChildren<Light>().enabled = !gameObject.GetComponentInChildren<Light>().enabled;
        } 
        if(GameEvent.isHiding)
            gameObject.GetComponentInChildren<Light>().enabled = false;
    }
}
