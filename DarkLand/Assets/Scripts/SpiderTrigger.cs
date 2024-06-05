using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderTrigger : MonoBehaviour
{
    [SerializeField] private GameObject spider;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
            spider.GetComponent<WaypointMover>().WakeUp();
        }
    }
    public void AddSpider(GameObject spider){
        this.spider = spider;
    }
}
