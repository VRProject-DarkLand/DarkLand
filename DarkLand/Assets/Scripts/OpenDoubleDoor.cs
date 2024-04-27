using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoubleDoor : MonoBehaviour
{
    private Tuple<Quaternion,Quaternion> open;
    private Tuple<Quaternion,Quaternion> close;
    private bool opened ;
    [SerializeField] private float left_rotation;
    [SerializeField] private float right_rotation;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    // Start is called before the first frame updatevoid Start()
    void Start(){
        opened = false;
        close  = Tuple.Create(left.transform.rotation, right.transform.rotation);
        open = Tuple.Create(close.Item1*Quaternion.Euler(0, left.transform.rotation.y+left_rotation, 0), close.Item2*Quaternion.Euler(0, right.transform.rotation.y+right_rotation, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
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
