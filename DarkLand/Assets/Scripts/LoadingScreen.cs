using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Image background; 
    // Start is called before the first frame update
    void Awake(){
        Messenger<float>.AddListener(GameEvent.LOADING_VALUE, UpdateLoadingBar);
         ChooseBackGround();
          loadingBar.value = 0;
    }
    void Start()
    {
       
    }

    private void ChooseBackGround(){
        if(sprites.Count != 0)
            background.sprite = sprites[Random.Range(0, sprites.Count)];
    }

    public void UpdateLoadingBar(float value){
        loadingBar.value = value;
    }

    void OnDestroy(){
          Messenger<float>.RemoveListener(GameEvent.LOADING_VALUE, UpdateLoadingBar);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
