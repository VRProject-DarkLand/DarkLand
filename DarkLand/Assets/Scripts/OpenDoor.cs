using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private bool opened ;
    // Start is called before the first frame update
    void Start()
    {
        opened = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open(){
        if(gameObject != null && !opened){
            Debug.Log("CIAO");
            gameObject.transform.rotation*=Quaternion.Euler(0, 90f, 0);
            opened = true;
        }
    }

    void OnTriggerEnter(Collider collider){
        Open();
        Debug.Log("mannaja di");
    }
}
