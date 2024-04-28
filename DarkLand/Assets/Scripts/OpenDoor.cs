using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Quaternion open;
    private Quaternion close;
    private bool opened ;
    private bool enteredInRange;

    [SerializeField] private float rotation = -90f;
    [SerializeField] private GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        if(door != null)
        {
            enteredInRange = false;
            opened = false;
            close  = door.transform.rotation;
            open = close*Quaternion.Euler(0, door.transform.rotation.y+rotation, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(enteredInRange){
            if(Input.GetKeyDown(KeyCode.E))
                ChangeState(); 
        } 
    }

    public void ChangeState(){
        if(door != null){
            door.transform.rotation = opened ? close : open; 
            opened = !opened;        
        }
    }

    void OnTriggerEnter(Collider collider){
        if(collider.tag == Settings.PLAYER_TAG)
            enteredInRange = true;

    }
    void OnTriggerExit(Collider collider){
        if(collider.tag == Settings.PLAYER_TAG)
            enteredInRange = false;  
    }

}
