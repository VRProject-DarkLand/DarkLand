using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PointerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get;private set;}
    private static int unlockValue = 0;
    // Start is called before the first frame update
    public void Startup(){
        Debug.Log("Cursor manager starting...");
        status = ManagerStatus.Started;
        Cursor.SetCursor(ResourceLoader.GetCursor(), Vector2.zero, CursorMode.Auto);
        //UnlockCursor();
    }

    public bool IsLock(){
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public void LockCursor(){
        unlockValue -=1;
        if(unlockValue <= 0){
            unlockValue = 0;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log(unlockValue);
        }
    }

    public void ForceLock(){
        unlockValue = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log(unlockValue);
    }

    public void UnlockCursor(){
        unlockValue +=1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log(unlockValue);
    }
}
