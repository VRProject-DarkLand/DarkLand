using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaryGirlTrigger : MonoBehaviour
{
    [SerializeField] private GameObject scaryGirl;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Settings.PLAYER_TAG)){
            scaryGirl.GetComponent<ScaryGirlAI>().WakeUp();
        }
    }
}
