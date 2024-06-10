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
    public IEnumerator SaveGame(){
        foreach(Transform t in _collectablePrototypes.transform){
            Destroy(t.GetComponent<Collectable>());
        }
        yield return null;
        Settings.gameData = new GameData();
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach(IDataPersistenceSave dataPersObj in _dataPersistenceObjects){
            
           dataPersObj?.SaveData();
           //Debug.Log(dataPersObj);
        }
        FileDataHandler.Save(_fileName);
        //Debug.Log("Saved data");
        Messenger.Broadcast(GameEvent.SAVE_FINISHED, MessengerMode.DONT_REQUIRE_LISTENER);
    }
    public void SetLoadedData(){
        _collectablePersistenceObject = FindAllCollectableObjects();
        //Debug.Log("Found " + _collectablePersistenceObject.Count +" collectables");
        // Dictionary<string, Collectable> toLoadObjects = new Dictionary<string, Collectable>();
        for(int i = 0; i < _collectablePersistenceObject.Count; ++i){
            if(_collectablePersistenceObject[i].transform.parent == null){
                //Debug.Log("Destroyed collectable WITH EMPTY PARENT" + _collectablePersistenceObject[i].name);
                Destroy(_collectablePersistenceObject[i].gameObject);
            }
            if(!_collectablePersistenceObject[i].transform.parent.CompareTag("Prototype")){
                Destroy(_collectablePersistenceObject[i].gameObject);
                //Debug.Log("Destroyed collectable " + _collectablePersistenceObject[i].name);
            }
        }
        GameObject objPrefab;
        for(int i = 0; i < Settings.gameData.collectableItemsPrefabs.Count; ++i){
            //Debug.Log("Positioning object with guid: " + Settings.gameData.collectableItemsPrefabs[i]);
            string prefabName = GetPrefabName(Settings.gameData.collectableItemsPrefabs[i]);

            Debug.Log("Searching for: " + prefabName +" name was "+ Settings.gameData.collectableItemsPrefabs[i]);
            objPrefab = _collectablePrototypes.transform.Find(prefabName).gameObject;
            Vector3 objPos= Settings.gameData.collectableItemsPosition[i];
            Vector3 objRot = Settings.gameData.collectableItemsRotation[i];
            Vector3 objScale = Settings.gameData.collectableItemsScale[i];
            GameObject obj = Instantiate(objPrefab, objPos, Quaternion.identity);
            Debug.Log("Restored object in scene");
            obj.transform.parent = _allCollectablesContainer.transform;
            obj.transform.localEulerAngles = objRot;
            obj.transform.localScale = objScale;
            obj.transform.name = Settings.gameData.collectableItemsNames[i];
        }
        
        GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG).GetComponent<FPSInput>().SetSaveData();
        //destroy all monsters in the scene and substitute them with those in the save file
        if(!GameEvent.exitingCurrentScene){
            _allMonsters = FindAllMonsters();
            foreach(GameObject monster in _allMonsters){
                //Debug.Log("Deleting scary girl");
                Destroy(monster);
            }
            _scaryGirlTriggers = FindAllScaryGirlTriggers();
            //Debug.Log("Found " +_scaryGirlTriggers.Count + " triggers");
            //restore scary girls
            GameObject scaryGirl;
            foreach(ScaryGirlAI.ScaryGirlSavingData data in Settings.gameData.scaryGirlsData){
                //Debug.Log("Creating scary girl");
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
            GameObject littleGirl;
            foreach(LittleGirlAI.LittleGirlSavingData data in Settings.gameData.littleGirlsData){
                //Debug.Log("Creating little girl");
                littleGirl = Instantiate(_monsterPrototypes._littleGirlPrefab);
                littleGirl.GetComponentInChildren<LittleGirlAI>().LoadFromData(data);
            }
            GameObject spider;
            List<SpiderTrigger> SpiderTriggers = FindAllSpiderTriggers();
            foreach(WaypointMover.SpiderData data in Settings.gameData.spidersData){
                //Debug.Log("Creating little girl");
                spider = Instantiate(_monsterPrototypes._spiderPrefab);
                spider.GetComponentInChildren<WaypointMover>().LoadFromData(data);
                foreach(SpiderTrigger t in SpiderTriggers){
                    t.AddSpider(spider.transform.GetChild(0).gameObject);
                    spider.GetComponentInChildren<WaypointMover>().AddSpiderTrigger(t);
                }
                //littleGirl.GetComponentInChildren<LittleGirlAI>().LoadFromData(data);
            }
            //open all boxes that were opened in the save
            Dictionary<string, bool> boxToState = new Dictionary<string, bool>();
            foreach(SlidingCrate.WeaponBoxData c in Settings.gameData.weaponBoxes){
                Debug.Log("Found box with state " + c.used);
                boxToState[c.name] = c.used;
            }
            List<SlidingCrate> boxes = FindAllBoxes();
            foreach(SlidingCrate box in boxes){
                if(boxToState[box.name]){
                    Debug.Log("Found open box");
                    box.ChangeState();
                }
            }
        }
        //set lights to off only when inside asylum
        if(ScenesController.instance._currentScene == Settings.ASYLUM_SCENE && !Settings.gameData.allLightsStatus){
            Messenger<bool>.Broadcast(GameEvent.OPERATE_ON_LIGHTS, false, MessengerMode.DONT_REQUIRE_LISTENER);
        }

        //Make a preliminary save of the new scene
        if(GameEvent.exitingCurrentScene){
            GameEvent.exitingCurrentScene = false;
            StartCoroutine(SaveGame());
        }
        
    }
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

    public GameObject CreateInventoryItem(string prefabName){
        GameObject objPrefab = null;
        Transform t =  _collectablePrototypes.transform.Find(prefabName);
        if(t != null){
            objPrefab = t.gameObject;
        }else
            objPrefab = _collectablePrototypes.transform.Find(GetPrefabName(prefabName)).gameObject;
       
        GameObject obj = Instantiate(objPrefab);
        //obj.name = objectName;
        //Debug.Log("Created inventory item in scene " + obj.name);

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
    private List<IDataPersistenceSave> FindAllDataPersistenceObjects(){
        IEnumerable<IDataPersistenceSave> data = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistenceSave>();
        return new List<IDataPersistenceSave>(data);
    }
    private List<Collectable> FindAllCollectableObjects(){
        IEnumerable<Collectable> data = FindObjectsOfType<MonoBehaviour>().OfType<Collectable>();
        return new List<Collectable>(data);
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
