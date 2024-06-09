using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    [SerializeField] private GameObject LoadingScreen;
    public string _currentScene = "";
    [SerializeField] public static ScenesController instance { get; private set;}
    void Awake(){
        if(instance == null){
            instance = this;
            LoadingScreen.SetActive(false);
            Messenger.AddListener(GameEvent.SAVE_FINISHED, SaveFinished);
        }

    }
    void  OnDestroy(){
        Messenger.RemoveListener(GameEvent.SAVE_FINISHED, SaveFinished);
    }
    private void SaveFinished(){
        if(GameEvent.exitingCurrentScene){
            //load data from lastSaving
            Loader.Load(Settings.LastSaving);
            Managers.Persistence.DeleteFile();
            //update lastSaving string
            string [] elements = Settings.LastSaving.Split('_');
            elements[0] = GameEvent.newScene;
            Settings.LastSaving  =  string.Join('_', elements);
            ChangeScene(GameEvent.newScene);
        }

    }
    public void ChangeScene(string scene){
        LoadingScreen.SetActive(true);
        Messenger.Broadcast(GameEvent.CHANGING_SCENE, MessengerMode.DONT_REQUIRE_LISTENER);
        StartCoroutine(LoadingScene(scene));
    }

    private IEnumerator LoadingScene(string scene){
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        while(!operation.isDone){
            Messenger<float>.Broadcast(GameEvent.LOADING_VALUE, operation.progress);
            yield return null;
        }
        _currentScene = scene;
    }

}
