using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(InteractableTrigger))]
public class OpenDoor : MonoBehaviour, IInteractableObject{
    private Quaternion open;
    private Quaternion close;
    private bool opened ;
    private bool isMoving;
    private float speed;
    private float timeCount;
    private bool canInteract;
    [SerializeField] private float rotation = -90f;
    [SerializeField] private GameObject door;
    // Start is called before the first frame update
    void Start(){
        if(door != null){
            canInteract = false;
            isMoving = false;
            opened = false;
            close  = door.transform.rotation;
            open = close*Quaternion.Euler(0, door.transform.rotation.y+rotation, 0);
        }
        timeCount = 0;
        speed = 1f;
    }

    // Update is called once per frame
    void Update(){
        if(isMoving){
            Quaternion begin = open;
            Quaternion end = close;
            if(opened){
                begin = close;
                end = open;
            }
            door.transform.rotation = Quaternion.Lerp(begin, end,  timeCount * speed);
            timeCount += Time.deltaTime;
            if(door.transform.rotation == end){
                isMoving = false;
                timeCount = 0;
            }
        }
    }

    public void ChangeState(){
        if(door != null){
            //door.transform.rotation = opened ? close : open; 
            opened = !opened;   
            isMoving = true;     
        }
    }

    public void Interact(){
        if(!isMoving)
            ChangeState();
    }
}
