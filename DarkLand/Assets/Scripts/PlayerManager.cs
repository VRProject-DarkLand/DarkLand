using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public int maxHealth {get; private set;}
    public int healthPackValue {get; private set;}
    public ManagerStatus status {get;private set;}

    public float fearLevel {get; private set;}
    // Start is called before the first frame update
    void Start()
    {
        Messenger.AddListener(GameEvent.PLAYER_DEAD, Die);
    }

    public void Startup(){
        Debug.Log("Player manager starting...");
        maxHealth = 100;
        healthPackValue = 2;
        fearLevel = 0f;
        status = ManagerStatus.Started;
    }

    private void Die(){
        Debug.Log("WE ROMINA");
    }

    void OnDestroy(){
        Messenger.RemoveListener(GameEvent.PLAYER_DEAD,Die);
    }


}
