using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Add me!!
public class Menu : MonoBehaviour
{
    [SerializeField] GameObject LoadMenu;
    [SerializeField] GameObject GamesContainer; 
    [SerializeField] GameObject MainMenu; 
    [SerializeField] GameObject SaveObject;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GameObject EmptyGames;
    [SerializeField] GameObject NewGamePanel;
    [SerializeField] TMP_InputField newGameName;
    [SerializeField] GameObject startNewGameButton;
    [SerializeField] SettingsController settings;
    private List<SaveMenuObject> saveObjects  = new();

    private  int selected = -1; 
    private List<string> savings = new List<string>();
    //{ "Ndria00_12-12-2024_16:59", "Ndria00_12-11-2024_16:59", "Ndria00_12-12-2014_16:59", "RTocco_14-12-2024_16:58"};
    public void Start(){
        foreach(string f in Directory.GetFiles(Settings.SAVE_DIR)){
            savings.Add(Path.GetFileName(f));
        }
    
    }
    public void OnLoadGame(){
        
        Settings.LastSaving = "";
        MainMenu.SetActive(false);
        LoadMenu.SetActive(true);
        EmptyGames.SetActive(false);
        foreach (Transform child in GamesContainer.transform) {
            Destroy(child.gameObject);
        }

        saveObjects.Clear();
        int i = 1;
        if(savings.Count == 0)
            EmptyGames.SetActive(true);
        foreach(string file in savings){ 
                GameObject save = Instantiate(SaveObject, GamesContainer.transform);
                SaveMenuObject saveMenu =  save.GetComponent<SaveMenuObject>();
                saveObjects.Add(saveMenu);
                saveMenu.SetInfo(i.ToString(), file);
                int index = i-1;
                AddEvent(save, EventTriggerType.PointerClick, e => SelectSave(index) );
                i++;
        }
    }

    public void OnBack(){
        MainMenu.SetActive(true);
        LoadMenu.SetActive(false);
        settings.HideAll();
        settings.ActivePanel(false);
        NewGamePanel.SetActive(false);
    }

    public void OnNewGame(){
        NewGamePanel.SetActive(true);
        startNewGameButton.SetActive(false);
        newGameName.text = "";
        Settings.LastSaving = "";
    }

    public void OnDeleteFile(){
        try{
            if(Settings.LastSaving != ""){
                
                #if UNITY_EDITOR
                UnityEditor.FileUtil.DeleteFileOrDirectory( Path.Combine(Settings.SAVE_DIR, Settings.LastSaving));
		        UnityEditor.AssetDatabase.Refresh();
                #else 
                File.Delete( Path.Combine(Settings.SAVE_DIR, Settings.LastSaving) );
		        #endif
                savings.Clear();
                foreach(string f in Directory.GetFiles(Settings.SAVE_DIR)){
                    savings.Add(Path.GetFileName(f));
                }
            }
        }
        catch(Exception ex){
            Debug.Log("Unable to delete file");
        }
        selected = -1;
        OnLoadGame();
    }

    public void OnSettingsClick() {
            MainMenu.SetActive(false);
             settings.ActivePanel(true);
    }


    private void SelectSave(int save) {
        Debug.Log(save);
        if(Settings.LastSaving ==  saveObjects[save].fileName){
            OnPlayButton(false);
        }else{
            Settings.LastSaving = saveObjects[save].fileName;
            saveObjects[save].Select();
            if(selected>=0) 
                saveObjects[selected].Deselect();
            selected = save;
        }
        
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (!trigger) { Debug.LogWarning("No EventTrigger component found!"); return; }
            var eventTrigger = new EventTrigger.Entry { eventID = type };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
    }

    public void OnValueChanged(string value){
        if(value.Trim().Length == 0){
            startNewGameButton.SetActive(false);
        }else 
            startNewGameButton.SetActive(true);

    }

    public void OnPlayButton(bool newGame)
    {
        if(!newGame){
            Loader.Load(Settings.LastSaving);
            
        }else{
            Settings.LastSaving = newGameName.text.Trim()+"_"+DateTime.Now.ToString("yyyy-MM-dd_HH_mm");

        }
        ScenesController.instance.ChangeScene(Settings.ASYLUM_SCENE);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    void Update(){
        if(LoadMenu.activeSelf){
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
                if(Settings.LastSaving != ""){
                    OnPlayButton(false);
                }
            }

            else if(Input.GetKeyDown(KeyCode.Delete)){
                OnDeleteFile();
            }

            if(Input.GetKeyDown(KeyCode.UpArrow)){
                if(selected <= 0){
                        selected = 0;
                }else{
                        saveObjects[selected].Deselect();
                        selected -=1;
                        //  Canvas.ForceUpdateCanvases();

                        // scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, ((Vector2)scrollRect.transform.InverseTransformPoint( scrollRect.content.position)
                        // - (Vector2)scrollRect.transform.InverseTransformPoint( saveObjects[selected].transform.position)).y);
                        scrollRect.verticalNormalizedPosition = 1f-selected*1f/(1f*(saveObjects.Count-1));       
                        Canvas.ForceUpdateCanvases();
                }
                Settings.LastSaving = saveObjects[selected].fileName;
                saveObjects[selected].Select();
                
            }
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                    if(selected >= saveObjects.Count-1){
                        selected = saveObjects.Count-1;
                    }
                    else{
                        if(selected >= 0)
                            saveObjects[selected].Deselect();
                        selected +=1;
                        scrollRect.verticalNormalizedPosition = 1f-selected*1f/(1f*(saveObjects.Count-1));    
                        Canvas.ForceUpdateCanvases();
                    }
                    Settings.LastSaving = saveObjects[selected].fileName;
                    saveObjects[selected].Select();
            }

        }
    }
}
