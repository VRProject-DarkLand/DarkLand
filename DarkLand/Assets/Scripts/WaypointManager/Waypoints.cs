using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    private void OnDrawGizmos() {
        foreach(Transform t in transform){
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, 1f);
        }
        Gizmos.color = Color.red;
        for (int i =0; i < transform.childCount -1; i++){
            Gizmos.DrawLine(transform.GetChild(i).position + new Vector3(0f,0.3f,0f),transform.GetChild(i + 1).position + new Vector3(0f,0.3f,0f));
        }
    }

    public Transform GetNextWaypoint(Transform currentWaypoint){
        if (currentWaypoint == null){
            return transform.GetChild(0);
        }
        else if (currentWaypoint.GetSiblingIndex() < transform.childCount - 1){
            return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
        }
        else{
            return null;
        }
    }
}
