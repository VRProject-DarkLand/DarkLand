using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public bool paused  {get; private set;}
    public Scene currentScene;

    // Start is called before the first frame update
    void Start()
    {
        Messenger.AddListener(GameEvent.PLAYER_DEAD,Pause);
    }

    public void Startup(){
        status = ManagerStatus.Started;
        paused = false;
        currentScene = SceneManager.GetActiveScene();
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

    public void OnRestart(){
        paused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene(currentScene.path);
    }

    void OnDestroy(){
        Messenger.RemoveListener(GameEvent.PLAYER_DEAD,Pause);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
