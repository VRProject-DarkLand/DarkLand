using System;
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
    private static HashSet<int> chasingSet = new();
    private int maxFear = 0;
    private int actualFear = 0;
    
    void Start()
    {
        Messenger.AddListener(GameEvent.PLAYER_DEAD, Die);
        StartCoroutine(ActualFearChange());
    }
    //setup player data to its defaults
    public void Startup(){
        maxHealth = 100;
        health = maxHealth;
        healthPackValue = 2;
        fearLevel = 0f;
        dead = false;
        chasingSet = new HashSet<int>();
        maxFear = 0;
        actualFear = 0;
        status = ManagerStatus.Started;

    }
    //set player data basing on the load
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

    public void AddEnemy(int enemy, int fear){
        if(chasingSet.Add(enemy)){
            ChangeFear(fear);
        }
    }
    public void AddFear(int fear){
        maxFear = Math.Clamp(maxFear+fear, 0, 100);
        actualFear = Math.Clamp(actualFear+fear, 0, maxFear);
        Messenger<int>.Broadcast(GameEvent.FEAR_CHANGED, actualFear);
    }

    public void RemoveEnemy(int enemy, int fear){
        if(chasingSet.Remove(enemy))
            ChangeFear(-fear);
    }

    public int GetChasingEnemies(){
        return chasingSet.Count;
    }

    private void ChangeFear(int fear){
        maxFear+=fear;
    }


    
    private void Die(){
        dead = true;
    }

    void OnDestroy(){
        Messenger.RemoveListener(GameEvent.PLAYER_DEAD,Die);
    }

    //used to gradually increase the current fear level of the player
    private IEnumerator ActualFearChange(){
        //actualFear = Mathf.RoundToInt(Mathf.Lerp(actualFear, maxFear, Time.deltaTime));
        int count = 0;
        Messenger<int>.Broadcast(GameEvent.FEAR_CHANGED, actualFear, MessengerMode.DONT_REQUIRE_LISTENER);
        while(!dead){
            //negative effects of fear
            Managers.AudioManager.PlayHearthBeat(actualFear>25);
            Managers.AudioManager.PlayWhisper(actualFear>90);
            if(maxFear>actualFear){
                actualFear += 1 ;
                count = 0;
                //notify the UI
                Messenger<int>.Broadcast(GameEvent.FEAR_CHANGED, actualFear, MessengerMode.DONT_REQUIRE_LISTENER);
                yield return new WaitForSeconds(0.5f);
            }else if(maxFear<actualFear){
                count += 1;
                actualFear -= 1;
                //notify the UI
                Messenger<int>.Broadcast(GameEvent.FEAR_CHANGED, actualFear);
                yield return count >10? new WaitForSeconds(0.25f) : new WaitForSeconds(0.5f);
            }
            else{
                yield return new WaitForSeconds(1f);
            }
        }     
    }

  

}
