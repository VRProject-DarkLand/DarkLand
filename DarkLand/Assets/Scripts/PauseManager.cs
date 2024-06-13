using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public bool paused  {get; private set;}
    public Scene currentScene;

    public bool settings; 

    // Start is called before the first frame update
    void Start()
    {
        Messenger.AddListener(GameEvent.PLAYER_DEAD,Pause);
        Messenger.AddListener(GameEvent.CHANGING_SCENE, Pause);
    }

    public void Startup(){
        status = ManagerStatus.Started;
        paused = false;
        settings = false;
        currentScene = SceneManager.GetActiveScene();
         AudioListener.pause = false;
    }

    public void OnClickResume(){
        paused = false;
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void OnEscResume(){
        if(!settings){
            OnClickResume();
        }
        Messenger<bool>.Broadcast(GameEvent.PAUSED, paused);
    }

    public void Pause(){
        paused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
        Messenger<bool>.Broadcast(GameEvent.PAUSED, paused,MessengerMode.DONT_REQUIRE_LISTENER);
    }

    public void OnRestart(){
        paused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        Settings.LoadedFromSave = true;
        SceneManager.LoadScene(currentScene.path);
    }

    void OnDestroy(){
        Messenger.RemoveListener(GameEvent.PLAYER_DEAD,Pause);
        Messenger.RemoveListener(GameEvent.CHANGING_SCENE, Pause);
    }

}
