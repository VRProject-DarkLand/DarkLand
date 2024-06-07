using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public int maxHealth {get; private set;}
    public int health {get; private set;}
    public Vector3 playerPosition {get; private set;}
    public Vector3 playerRotation {get; private set;}
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
        health = maxHealth;
        healthPackValue = 2;
        fearLevel = 0f;
        dead = false;
        status = ManagerStatus.Started;

    }
    public void SetLoadGameData(){
        health = Settings.gameData.playerHealth;
        Vector3 pPos = Settings.gameData.playerPosition;
        playerPosition = new Vector3(pPos.x, pPos.y, pPos.z);
        Vector3 pRot = Settings.gameData.playerRotation;
        playerRotation = new Vector3(pRot.x, pRot.y, pRot.z);
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
