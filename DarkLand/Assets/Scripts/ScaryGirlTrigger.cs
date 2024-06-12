using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScaryGirlTrigger : MonoBehaviour
{
    
    private List<GameObject> scaryGirls;
    [SerializeField] private GameObject[] sceneScaryGirls;

    void Start(){
        scaryGirls = new List<GameObject>();
        if(!Settings.LoadedFromSave || GameEvent.OpenedSceneDoor){
            foreach(var sceneScaryGirl in sceneScaryGirls)
                scaryGirls.Add(sceneScaryGirl);
        }
    }
    private void OnTriggerEnter(Collider other) {
        Debug.Log("TriggerScaryGirl");
        foreach(GameObject girl in scaryGirls){
            if (other.CompareTag(Settings.PLAYER_TAG)){
                girl.GetComponent<ScaryGirlAI>().WakeUp();
            }
        }
    }
    public void AddScaryGirl(GameObject girl) {
        scaryGirls.Add(girl);
    }
    public void RemoveScaryGirl(GameObject girl) {
        scaryGirls.Remove(girl);
    }
}
