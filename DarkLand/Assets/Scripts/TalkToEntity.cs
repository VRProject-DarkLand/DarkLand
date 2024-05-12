// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// public class TalkToEntity : MonoBehaviour
// {
//     public void OnTriggerEnter(Collider other){
//         Messenger<string>.Broadcast(GameEvent.DialogTypes.PLAYER_ENTERED_NPC_RANGE.ToString(), gameObject.name);
//         //Debug.Log("Player entered NPC " + gameObject.name);
//     }
//     public void OnTriggerExit(Collider other){
//         Messenger<string>.Broadcast(GameEvent.DialogTypes.PLAYER_EXIT_NPC_RANGE.ToString(), gameObject.name);
//         //Debug.Log("Player exit NPC " + gameObject.name);
//     }
// }
