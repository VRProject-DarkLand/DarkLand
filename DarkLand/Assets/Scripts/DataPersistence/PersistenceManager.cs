using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceManager : MonoBehaviour, IGameManager{
    public ManagerStatus status {get;private set;}

    private string _fileName;
    private List<IDataPersistenceSave> _dataPersistenceObjects = null;
    private List<GameObject> _allMonsters = null;
    private List<ScaryGirlTrigger> _scaryGirlTriggers = null;
    private List<Collectable> _collectablePersistenceObject = null;
    private FileDataHandler _dataHandler;
    [SerializeField] private GameObject _collectablePrototypes;
    [SerializeField] private GameObject _allCollectablesContainer;
    [SerializeField] private MonsterPrototypeHandler _monsterPrototypes;
     
    public void Startup(){
        _fileName = Settings.LastSaving;
        if(Settings.LoadedFromSave){
            GameObject wakeUpCamera = GameObject.Find("WakeUpCamera");
            if(wakeUpCamera != null){
                Destroy(wakeUpCamera);
            }
        }
        status = ManagerStatus.Started;
    }
    public GameObject GetAllCollectablesContainer(){
        return _allCollectablesContainer;
    }
    //get all data persistent objects and write their data to
    // a new instance of GameData
    public IEnumerator SaveGame(){

        yield return null;
        Settings.gameData = new GameData();
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach(IDataPersistenceSave dataPersObj in _dataPersistenceObjects){
            if(dataPersObj != null)
                dataPersObj.SaveData();
        }
        FileDataHandler.Save(_fileName);
        Messenger.Broadcast(GameEvent.SAVE_FINISHED, MessengerMode.DONT_REQUIRE_LISTENER);
    }
    public void SetLoadedData(){
        _collectablePersistenceObject = FindAllCollectableObjects();
        if(!GameEvent.OpenedSceneDoor){
            //destroy all non-prototype collectable objects in the scene
            for(int i = 0; i < _collectablePersistenceObject.Count; ++i){
                if(_collectablePersistenceObject[i].transform.parent == null){
                    Destroy(_collectablePersistenceObject[i].gameObject);
                }
                if(!_collectablePersistenceObject[i].transform.parent.CompareTag("Prototype")){
                    Destroy(_collectablePersistenceObject[i].gameObject);
                }
            }
        }
        //instantiate collectable objects from prototypes and restore their data (just position and rotation) 
        GameObject objPrefab;
        for(int i = 0; i < Settings.gameData.collectableItemsPrefabs.Count; ++i){
            string prefabName = GetPrefabName(Settings.gameData.collectableItemsPrefabs[i]);

            objPrefab = _collectablePrototypes.transform.Find(prefabName).gameObject;
            Vector3 objPos= Settings.gameData.collectableItemsPosition[i];
            Vector3 objRot = Settings.gameData.collectableItemsRotation[i];
            Vector3 objScale = Settings.gameData.collectableItemsScale[i];
            GameObject obj = Instantiate(objPrefab, objPos, Quaternion.identity);
            obj.transform.parent = _allCollectablesContainer.transform;
            obj.transform.localEulerAngles = objRot;
            obj.transform.localScale = objScale;
            obj.transform.name = Settings.gameData.collectableItemsNames[i];
        }
        //restore player data
        GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG).GetComponent<FPSInput>().SetSaveData();
        //destroy all monsters in the scene and substitute them with those in the save file
        if(!GameEvent.exitingCurrentScene){
            _allMonsters = FindAllMonsters();
            foreach(GameObject monster in _allMonsters){
                LittleGirlAI ai = monster.GetComponentInChildren<LittleGirlAI>();
                WaypointMover mover = monster.GetComponentInChildren<WaypointMover>();
                //do not destroy little girl - we decided not to persist such a monster
                //do not destroy spiders in asylum - see comment below
                if(ai == null){
                    if(mover == null)
                        Destroy(monster);
                    else{
                        if(SceneManager.GetActiveScene().name != Settings.ASYLUM_NAME){
                            Destroy(monster);
                        }
                    }
                }
            }
            //create scary girls from saving and make the double register of scary girls to their triggers
            //and of triggers to their scary girls
            _scaryGirlTriggers = FindAllScaryGirlTriggers();
            GameObject scaryGirl;
            foreach(ScaryGirlAI.ScaryGirlSavingData data in Settings.gameData.scaryGirlsData){
                if(!data.dead){
                    scaryGirl = Instantiate(_monsterPrototypes._scaryGirlPrefab);
                    ScaryGirlAI ai = scaryGirl.GetComponentInChildren<ScaryGirlAI>();
                    foreach(ScaryGirlTrigger t in _scaryGirlTriggers){
                        t.AddScaryGirl(scaryGirl.transform.GetChild(0).gameObject);
                        ai.AddScaryGirlTrigger(t);
                    }
                    ai.LoadFromData(data);
                }
            }
            //we decided not to load spiders in asylum. Spiders are too complex and require references to the scene to work
            if(SceneManager.GetActiveScene().name != Settings.ASYLUM_NAME){
                //create and set data to spiders
                GameObject spider;
                List<SpiderTrigger> SpiderTriggers = FindAllSpiderTriggers();
                foreach(WaypointMover.SpiderData data in Settings.gameData.spidersData){
                    spider = Instantiate(_monsterPrototypes._spiderPrefab);
                    spider.GetComponentInChildren<WaypointMover>().LoadFromData(data);
                    foreach(SpiderTrigger t in SpiderTriggers){
                        t.AddSpider(spider.transform.GetChild(0).gameObject);
                        spider.GetComponentInChildren<WaypointMover>().AddSpiderTrigger(t);
                    }
                }
            }

            //open all boxes that were opened in the save
            //this is the only restore operation needed for boxes
            //NOTE: if there are collectables that were left not picked from the player
            //inside the boxes are restored by the code that restores collectables by
            //creating them from prototypes
            Dictionary<string, bool> boxToState = new Dictionary<string, bool>();
            foreach(SlidingCrate.WeaponBoxData c in Settings.gameData.weaponBoxes){
                boxToState[c.name] = c.used;
            }
            List<SlidingCrate> boxes = FindAllBoxes();
            //open box
            foreach(SlidingCrate box in boxes){
                if(boxToState[box.name]){
                    box.ChangeState();
                }
            }
        }
        //set lights to off only when inside asylum
        if(SceneManager.GetActiveScene().name == Settings.ASYLUM_NAME && !Settings.gameData.allLightsStatus){
            Messenger<bool>.Broadcast(GameEvent.OPERATE_ON_LIGHTS, false, MessengerMode.DONT_REQUIRE_LISTENER);
        }

        //Make a preliminary save of the new scene
        //it is used to keep track of the inventory, but does not save monsters
        //since monsters in the new scene have to be kept as they are from the scene
        //NOTE: another save is made when the player spawns inside the forest
        if(GameEvent.exitingCurrentScene){
            GameEvent.exitingCurrentScene = false;
            StartCoroutine(SaveGame());
        }
        
    }
    //used to allow having more that one object with the same prefab
    private string GetPrefabName(string name){
        string prefabName = name;    
        if(prefabName.Contains("key", System.StringComparison.OrdinalIgnoreCase)){
            prefabName = "Key";
        }
        if(prefabName.Contains("letter", System.StringComparison.OrdinalIgnoreCase)){
            prefabName = "Letter";
        }
        return prefabName;
    }

    //creates an item that was saved in the inventory
    //inventory is restored by creating objects that were collected 
    //in the last save and then collecting them 
    public GameObject CreateInventoryItem(string prefabName){
        GameObject objPrefab = null;
        Transform t =  _collectablePrototypes.transform.Find(prefabName);
        if(t != null){
            objPrefab = t.gameObject;
        }else
            objPrefab = _collectablePrototypes.transform.Find(GetPrefabName(prefabName)).gameObject;
       
        GameObject obj = Instantiate(objPrefab);

        return obj;
    }
       public void DeleteFile(){
        try{
            if(Settings.LastSaving != ""){
                
                #if UNITY_EDITOR
                UnityEditor.FileUtil.DeleteFileOrDirectory( Path.Combine(Settings.SAVE_DIR, Settings.LastSaving));
		        UnityEditor.AssetDatabase.Refresh();
                #else 
                File.Delete( Path.Combine(Settings.SAVE_DIR, Settings.LastSaving) );
		        #endif
            }
        }
        catch{
            Debug.Log("Unable to delete file");
        }
    }
    //used to get all persistent objects before save
    private List<IDataPersistenceSave> FindAllDataPersistenceObjects(){
        IEnumerable<IDataPersistenceSave> data = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistenceSave>();
        return new List<IDataPersistenceSave>(data);
    }
    //The following methods are used only upon load such that the items in the scene can be restored
    private List<Collectable> FindAllCollectableObjects(){
        return _allCollectablesContainer.GetComponentsInChildren<Collectable>().ToList();
    }
    
    private List<ScaryGirlTrigger> FindAllScaryGirlTriggers(){
        IEnumerable<ScaryGirlTrigger> data = FindObjectsOfType<MonoBehaviour>().OfType<ScaryGirlTrigger>();
        return new List<ScaryGirlTrigger>(data);
    }
    private List<SpiderTrigger> FindAllSpiderTriggers(){
        IEnumerable<SpiderTrigger> data = FindObjectsOfType<MonoBehaviour>().OfType<SpiderTrigger>();
        return new List<SpiderTrigger>(data);
    }
    private List<GameObject> FindAllMonsters(){
        IEnumerable<GameObject> data = GameObject.FindGameObjectsWithTag(Settings.ENEMY_TAG);
        return new List<GameObject>(data);
    }
    private List<SlidingCrate> FindAllBoxes(){
        IEnumerable<SlidingCrate> data = FindObjectsOfType<MonoBehaviour>().OfType<SlidingCrate>();
        return new List<SlidingCrate>(data);
    }
}
