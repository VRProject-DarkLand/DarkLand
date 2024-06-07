using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUp : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera wakeUpCamera;
    [SerializeField] private GameObject player;


    public void wakeUp()
    {
        playerCamera.enabled = true;
        wakeUpCamera.enabled = false;
        player.GetComponent<FPSInput>().enabled = true;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<MouseLook>().enabled = true;
        Destroy(gameObject);
    }
}
