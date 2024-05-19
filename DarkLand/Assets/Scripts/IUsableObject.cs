using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUsableObject: MonoBehaviour{
    public abstract void Use();
    public abstract void Select();
    public abstract void Deselect();
    public abstract bool IsDummy();
}
