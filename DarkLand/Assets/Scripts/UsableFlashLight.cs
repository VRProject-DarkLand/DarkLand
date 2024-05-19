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
        gameObject.transform.localPosition = new Vector3(0.3f, -0.2f, 0.4f);
        //gameObject.transform.position = 
        //set torch visible
    }

    public override void Use(){
        Debug.Log("Changing light status");
        if(!GameEvent.isInDialog){
          gameObject.GetComponent<Light>().enabled = !gameObject.GetComponent<Light>().enabled;
        } 
        if(GameEvent.isHiding)
            gameObject.GetComponent<Light>().enabled = false;
    }
}
