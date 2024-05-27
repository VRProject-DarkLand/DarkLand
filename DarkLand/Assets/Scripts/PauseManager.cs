using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class PauseManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public bool paused  {get; private set;}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Startup(){
        status = ManagerStatus.Started;
        paused = false;
    }

    public void OnClickResume(){
        paused = false;
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void OnEscResume(){
        OnClickResume();
        Messenger<bool>.Broadcast(GameEvent.PAUSED, paused);
    }

    public void Pause(){
        paused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
        Messenger<bool>.Broadcast(GameEvent.PAUSED, paused);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
