using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingInteractable : IInteractableObject
{
    private bool isHiding;
    private bool isMoving;
    private GameObject player;
    private Vector3 initialPos ;
    private Quaternion initialRot;
    private float speed;
    private float timeCount;

    // Start is called before the first frame update
    void Start()
    {
        isHiding = false;
        player = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG);
        timeCount = 0;
        speed = 0.5f;
        initialPos = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator AnimateHiding(){
        //Messenger<bool>.Broadcast(GameEvent.IS_HIDING, !isHiding);
        
        Vector3 begin = initialPos;
        Vector3 end = gameObject.transform.position;
        Quaternion beginRot = initialRot;
        Quaternion endRot = Quaternion.FromToRotation(player.transform.forward, gameObject.transform.forward);
        if(isHiding){
            begin = gameObject.transform.position;
            end = initialPos;
            beginRot=player.transform.rotation;
            endRot = initialRot;
        }
        endRot = Quaternion.Euler(beginRot.x, endRot.y, beginRot.z);
        while(isMoving){
            player.transform.position = Vector3.Lerp(begin, end,  timeCount * speed);
            player.transform.rotation = Quaternion.Lerp(beginRot, endRot, speed*timeCount);
            timeCount += Time.deltaTime;
            if(player.transform.position == end && player.transform.rotation == endRot){
                isMoving = false;
                timeCount = 0;
            }
            yield return null;
        }
        isHiding = !isHiding;
        
        //Messenger<bool>.Broadcast(GameEvent.IS_HIDING, isHiding);
        //message with isHiding
    }

    public override void Interact(){
        if(isMoving)
            return;
        isMoving = true;
        if(!isHiding){   
            initialPos = player.transform.position;
            initialRot = player.transform.rotation;
        }
        Debug.Log("initial pos "+ initialPos +" "+Time.frameCount );
        GameEvent.isHiding=!isHiding;
        StartCoroutine(AnimateHiding());

    }
    
}
