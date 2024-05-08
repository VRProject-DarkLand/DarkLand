using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Quaternion open;
    private Quaternion close;
    private bool opened ;
    private bool isMoving;
    private bool enteredInRange;
    private float speed;
    private float timeCount;
    private Transform playerTransform;
    [SerializeField] private float rotation = -90f;
    [SerializeField] private GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        if(door != null)
        {
            enteredInRange = false;
            isMoving = false;
            opened = false;
            close  = door.transform.rotation;
            open = close*Quaternion.Euler(0, door.transform.rotation.y+rotation, 0);
        }
        timeCount = 0;
        speed = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(enteredInRange&& !isMoving){
            if(Input.GetKeyDown(KeyCode.E) ){
                Vector3 directionToPlayer =  door.transform.position - playerTransform.position;
                if (Vector3.Dot(playerTransform.forward, directionToPlayer)>0 ){
                    ChangeState();
                 }
            }
        } 
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

    void OnTriggerEnter(Collider collider){
        if(collider.tag == Settings.PLAYER_TAG){
            enteredInRange = true;
            playerTransform = collider.gameObject.transform;
        }

    }
    void OnTriggerExit(Collider collider){
        if(collider.tag == Settings.PLAYER_TAG)
            enteredInRange = false;    
        }

}
