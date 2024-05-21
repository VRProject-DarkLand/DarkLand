using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUsableObject: MonoBehaviour{
    public abstract void Use();
    
    public virtual void SecondaryUse(){
        
    }
    public abstract void Select();
    public virtual void Deselect(){
        gameObject.SetActive(false);
    }
    public abstract bool IsDummy();
}
