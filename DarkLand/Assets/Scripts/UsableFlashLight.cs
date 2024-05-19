using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableFlashLight : IUsableObject{
    
    
    public override void Deselect(){
        //torchHandler.
        //turn off and deselect
    }

    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.66f);
        //gameObject.transform.position = 
        //set torch visible
    }

    public override void Use(){
        if(!GameEvent.isInDialog){
            Debug.Log("Changing light status");
            gameObject.GetComponentInChildren<Light>().enabled = !gameObject.GetComponentInChildren<Light>().enabled;
        } 
        if(GameEvent.isHiding)
            gameObject.GetComponentInChildren<Light>().enabled = false;
    }
}
