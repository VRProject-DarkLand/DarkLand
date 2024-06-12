using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiderTrigger : MonoBehaviour
{
    private List<GameObject> spiders;
    [SerializeField] private GameObject[] _sceneSpider;
    void Start(){
        spiders = new List<GameObject>();
        if(!Settings.LoadedFromSave || GameEvent.OpenedSceneDoor){
            foreach(GameObject obj in _sceneSpider)
            {
                spiders.Add(obj);
            }
        }
    }
    private void OnTriggerEnter(Collider other) {

        foreach(GameObject spider in spiders){
            if (other.CompareTag(Settings.PLAYER_TAG)){
                spider.GetComponent<WaypointMover>().WakeUp();
                Debug.Log("SpiderMan");
            }
        }
    }
    public void AddSpider(GameObject spider){
        spiders.Add(spider);
    }
    public void RemoveSpider(GameObject spider){
        spiders.Remove(spider);
    }
}
