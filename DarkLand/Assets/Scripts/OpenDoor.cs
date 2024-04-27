using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Quaternion open;
    private Quaternion close;
    private bool opened ;
    [SerializeField] private GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        if(door != null)
        {
            opened = false;
            close  = door.transform.rotation;
            open = close*Quaternion.Euler(0, door.transform.rotation.y-90f, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(){
        if(door != null){
            door.transform.rotation = opened ? close : open; 
            opened = !opened;        
        }
    }

    void OnTriggerEnter(Collider collider){
        if(Input.GetKeyDown(KeyCode.E)){
            ChangeState();
        } 
    }
    void OnTriggerStay(Collider collider){
        if(Input.GetKeyDown(KeyCode.E)){
            ChangeState();
        }  
    }

}
