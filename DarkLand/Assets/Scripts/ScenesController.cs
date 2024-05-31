using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    [SerializeField] private GameObject LoadingScreen;

    [SerializeField] public static ScenesController instance { get; private set;}

    void Awake(){
        if(instance == null){
            instance = this;
            LoadingScreen.SetActive(false);
        }
    }


    public void ChangeScene(string scene){
        LoadingScreen.SetActive(true);
        StartCoroutine(LoadingScene(scene));

    }

    private IEnumerator LoadingScene(string scene){
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        while(!operation.isDone){
            Messenger<float>.Broadcast(GameEvent.LOADING_VALUE, operation.progress);
            yield return null;
        }
    }

}
