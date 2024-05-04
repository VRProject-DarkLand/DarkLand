using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenDoubleDoor : MonoBehaviour
{
    private Tuple<Quaternion,Quaternion> open;
    private Tuple<Quaternion,Quaternion> close;
    private bool opened ;
    private bool enteredInRange;
    private Transform playerTransform;
    [SerializeField] private float left_rotation;
    [SerializeField] private float right_rotation;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    // Start is called before the first frame updatevoid Start()
    void Start(){
        opened = false;
        enteredInRange = false;
        close  = Tuple.Create(left.transform.rotation, right.transform.rotation);
        open = Tuple.Create(close.Item1*Quaternion.Euler(0, left.transform.rotation.y+left_rotation, 0), close.Item2*Quaternion.Euler(0, right.transform.rotation.y+right_rotation, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if(enteredInRange){
            if(Input.GetKeyDown(KeyCode.E)){
                Vector3 directionToPlayerLeft =  left.transform.position - playerTransform.position;
                Vector3 directionToPlayerRight =  right.transform.position - playerTransform.position;
                if (Mathf.Max(Vector3.Dot(playerTransform.forward, directionToPlayerLeft), Vector3.Dot(playerTransform.forward, directionToPlayerRight) )>0 ){
                    ChangeState(); 
                }
            }
        }
    }

    public void ChangeState(){
        if(left != null && right != null){
                if(opened){
                    left.transform.rotation = close.Item1 ;
                    right.transform.rotation = close.Item2 ;
                }else{
                    left.transform.rotation = open.Item1 ;
                    right.transform.rotation = open.Item2 ;
                }
                opened = !opened;     
        }
    }

    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag == Settings.PLAYER_TAG)
            enteredInRange = true;
            playerTransform = collider.gameObject.transform;
    }

    void OnTriggerExit(Collider collider){
        if(collider.gameObject.tag == Settings.PLAYER_TAG)
            enteredInRange = false;
        
    }
}
