using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class HidingInteractable : IInteractableObject{
    private bool isHiding;
    public  bool isMoving;
    private GameObject player;
    private Vector3 exitPos ;
    private float speed;
    private float timeCount;
    [SerializeField] private GameObject quad;
    private Vector3 offQuadPosition;

    void Start(){
        isHiding = false;
        interactableTrigger = GetComponent<InteractableTrigger>();
        interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.HIDE);
        player = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG);
        timeCount = 0;
        speed = 0.85f;
        exitPos = gameObject.transform.position+gameObject.transform.forward;
        quad.SetActive(false);
        offQuadPosition = quad.transform.localPosition;
    }

    // Update is called once per frame
    void Update(){
        
    }

    private IEnumerator AnimateHiding(){
        quad.SetActive(true);
        Vector3 begin = player.transform.position;
        Vector3 end = gameObject.transform.position;

        Quaternion beginRot = player.transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(transform.forward);
        
        Vector3 startQuad = offQuadPosition;
        Vector3 endQuad = new Vector3(0f, startQuad.y, startQuad.z);
        if(isHiding){
            begin = gameObject.transform.position;
            end = exitPos;
            beginRot=endRot;
            startQuad = quad.transform.localPosition;
            endQuad = offQuadPosition;
        }
        //endRot = Quaternion.Euler(beginRot.x, endRot.y, beginRot.z);
        if(!isHiding){
            Messenger<bool>.Broadcast(GameEvent.IS_HIDING, isHiding, MessengerMode.DONT_REQUIRE_LISTENER);
            isMoving = true;
            while(isMoving){
                RotateAndMovePlayer(begin, end, beginRot, endRot);
                yield return null;
            }

            isMoving = true;
            while(isMoving){
                SlideQuad(startQuad, endQuad);
                yield return null;
            }
        }
        else{
            isMoving = true;
            while(isMoving){
                SlideQuad(startQuad, endQuad);
                yield return null;
            }
            quad.SetActive(false);

            isMoving = true;
            while(isMoving){
                RotateAndMovePlayer(begin, end, beginRot, endRot);
                yield return null;
            }
            Messenger<bool>.Broadcast(GameEvent.IS_HIDING, isHiding, MessengerMode.DONT_REQUIRE_LISTENER);
            
        }
        isHiding = !isHiding;
    }

    void RotateAndMovePlayer(Vector3 begin, Vector3 end, Quaternion beginRot, Quaternion endRot){
        player.transform.position = Vector3.Lerp(begin, end,  timeCount * speed);
        player.transform.rotation = Quaternion.Lerp(beginRot, endRot, speed*timeCount);
        //player.transform.LookAt(lookToMe.transform);
        timeCount += Time.deltaTime;
        if(player.transform.position == end){
            isMoving = false;
            timeCount = 0;
        } 
    }
    void SlideQuad(Vector3 startQuad, Vector3 endQuad){
        quad.transform.localPosition = Vector3.Lerp(startQuad, endQuad,  timeCount * speed * 4f);
        timeCount += Time.deltaTime;
        if(quad.transform.localPosition == endQuad){
            isMoving = false;
            timeCount = 0;
        }
    }

    public override bool CanInteract()
    {
        return  Managers.Player.GetChasingEnemies() == 0;
    }

    public override void Interact(){
        if(!CanInteract())
            return;
        if(isMoving)
            return;
        isMoving = true;
        if(isHiding)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.HIDE);
        else 
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.UNHIDE);
        GameEvent.isHiding=!isHiding;
        
        StartCoroutine(AnimateHiding());
        
    }   
}