using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableGun : IUsableObject{

    private Animator _animator;

    public override void Deselect(){
        
    }

    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        gameObject.SetActive(true);
    }

    public override void Use(){
        
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
