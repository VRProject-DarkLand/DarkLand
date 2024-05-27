using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUsableObject: MonoBehaviour{
    [SerializeField] protected RuntimeAnimatorController _controller = null;
    protected Animator _animator;
    [SerializeField] protected Vector3 _handlingPosition = Vector3.zero;
    [SerializeField] protected Vector3 _handlingRotation = Vector3 .zero;
    public abstract void Use();
    
    public virtual void SecondaryUse(){
        
    }

    public virtual void Collected(){
        
    }
    public virtual void UndoSecondaryUse(){
        
    }
    public abstract void Select();
    public virtual void Deselect(){
        gameObject.SetActive(false);
    }
    public abstract bool IsDummy();
    public virtual void Position(){

    }
}
