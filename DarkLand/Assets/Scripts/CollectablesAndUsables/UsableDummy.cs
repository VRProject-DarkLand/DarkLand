using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableDummy : IUsableObject
{
    public override void Deselect(){
        return;
    }

    public override bool IsDummy(){
        return true;
    }

    public override void Select(){
        return;
    }

    public override void Use(){
        return;
    }
}
