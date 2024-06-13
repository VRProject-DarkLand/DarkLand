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
        bosses.Add(boss);
    }
    private void OnTriggerEnter(Collider other) {
        foreach(GameObject boss in bosses){
            if (other.CompareTag(Settings.PLAYER_TAG)){
                boss.GetComponent<CreepHorrorCreature>().WakeUp();
            }
        }
    }
    public void AddFinalBoss(GameObject girl) {
        bosses.Add(girl);
    }
    public void RemoveFinalBoss(GameObject girl) {
        bosses.Remove(girl);
    }
}
