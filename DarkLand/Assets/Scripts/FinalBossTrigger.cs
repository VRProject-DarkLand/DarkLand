using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FinalBossTrigger : MonoBehaviour
{
    
    private List<GameObject> bosses;
    [SerializeField] private GameObject boss;

    void Start(){
        bosses = new List<GameObject>();
        if(!Settings.LoadedFromSave)
            bosses.Add(boss);
    }
    private void OnTriggerEnter(Collider other) {
        foreach(GameObject girl in bosses){
            if (other.CompareTag(Settings.PLAYER_TAG)){
                girl.GetComponent<CreepHorrorCreature>().WakeUp();
            }
        }
    }
    public void AddScaryGirl(GameObject girl) {
        bosses.Add(girl);
    }
    public void RemoveScaryGirl(GameObject girl) {
        bosses.Remove(girl);
    }
}
