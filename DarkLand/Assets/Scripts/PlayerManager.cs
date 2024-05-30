using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public int maxHealth {get; private set;}
    public int health {get; private set;}
    public int healthPackValue {get; private set;}
    public ManagerStatus status {get;private set;}
    public bool dead {get; private set;}
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
        dead = false;
        status = ManagerStatus.Started;
        if(Settings.gameData != null){
            LoadGameData();
        }else{
            SetNewGameData();
        }
    }
    private void LoadGameData(){
        health = Settings.gameData.playerHealth;
    }
    private void SetNewGameData(){
        health = maxHealth;
    }
    
    private void Die(){
        dead = true;
    }

    void OnDestroy(){
        Messenger.RemoveListener(GameEvent.PLAYER_DEAD,Die);
    }


}
