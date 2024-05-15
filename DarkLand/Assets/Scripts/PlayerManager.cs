using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public int health {get; private set;}
    public int maxHealth {get; private set;}
    public int healthPackValue {get; private set;}
    public int barValueDamage {get; private set;}
    public ManagerStatus status {get;private set;}

    public float fearLevel {get; private set;}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Startup(){
        Debug.Log("Player manager starting...");
        health = 5;
        maxHealth = 100;
        healthPackValue = 2;
        barValueDamage = maxHealth /health;
        fearLevel = 0f;
        status = ManagerStatus.Started;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
