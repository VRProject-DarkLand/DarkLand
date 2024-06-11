using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScaryGirlTrigger : MonoBehaviour
{
    
    private List<GameObject> scaryGirls;
    [SerializeField] private GameObject sceneScaryGirl;

    void Start(){
        scaryGirls = new List<GameObject>();
        if(!Settings.LoadedFromSave)
            scaryGirls.Add(sceneScaryGirl);
    }
    private void OnTriggerEnter(Collider other) {
        Debug.Log("TriggerScaryGirl");
        foreach(GameObject girl in scaryGirls){
            if (other.CompareTag(Settings.PLAYER_TAG)){
                girl.GetComponent<ScaryGirlAI>().WakeUp();
                Debug.Log("ADSAD");
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
