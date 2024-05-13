using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingInteractable : IInteractableObject
{
    private bool isHiding;
    public  bool isMoving;
    private GameObject player;
    private Vector3 exitPos ;
    private float speed;
    private float timeCount;

    // Start is called before the first frame update
    void Start()
    {
        isHiding = false;
        player = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG);
        timeCount = 0;
        speed = 0.5f;
        exitPos = gameObject.transform.position+gameObject.transform.forward;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator AnimateHiding(){
        //Messenger<bool>.Broadcast(GameEvent.IS_HIDING, !isHiding);
        
        Vector3 begin = player.transform.position;
        Vector3 end = gameObject.transform.position;
        Quaternion beginRot = player.transform.rotation;
        Quaternion endRot = Quaternion.FromToRotation(player.transform.forward, gameObject.transform.forward);
        if(isHiding){
            begin = gameObject.transform.position;
            end = exitPos;
            beginRot=endRot;
        }
        endRot = Quaternion.Euler(beginRot.x, endRot.y, beginRot.z);
        while(isMoving){
            player.transform.position = Vector3.Lerp(begin, end,  timeCount * speed);
            player.transform.rotation = Quaternion.Lerp(beginRot, endRot, speed*timeCount);
            timeCount += Time.deltaTime;
            if(player.transform.position == end ){
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
        Debug.Log("initial pos "+ exitPos +" "+Time.frameCount );
        GameEvent.isHiding=!isHiding;
        StartCoroutine(AnimateHiding());

    }
    
}
