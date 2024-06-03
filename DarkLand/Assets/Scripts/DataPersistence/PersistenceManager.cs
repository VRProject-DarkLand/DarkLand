using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PersistenceManager : MonoBehaviour, IGameManager{
    public ManagerStatus status {get;private set;}

    private string _fileName;
    private List<IDataPersistenceSave> _dataPersistenceObjects = null;
    private List<Collectable> _collectablePersistenceObject = null;
    private FileDataHandler _dataHandler;
    [SerializeField] private GameObject _collectablePrototypes;
    [SerializeField] private GameObject _allCollectablesContainer;
    
    public void Startup(){
        _fileName = Settings.LastSaving;
        status = ManagerStatus.Started;
        //find contained of prefabs for all objects that can be saved
        _collectablePrototypes = GameObject.Find("CollectablePrototype");
    }

    public void NewGame(){
        Debug.Log("New game creation");
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
           Debug.Log(dataPersObj);
        }
        FileDataHandler.Save(_fileName);
        Debug.Log("Saved data");
    }
    public void SetLoadedData(){
        _collectablePersistenceObject = FindAllCollectableObjects();
        //Debug.Log("Found " + _collectablePersistenceObject.Count +" collectables");
        // Dictionary<string, Collectable> toLoadObjects = new Dictionary<string, Collectable>();
        for(int i = 0; i < _collectablePersistenceObject.Count; ++i){
            if(_collectablePersistenceObject[i].transform.parent == null){
                Debug.Log("Destroyed collectable WITH EMPTY PARENT" + _collectablePersistenceObject[i].name);
                Destroy(_collectablePersistenceObject[i].gameObject);
            }
            if(!_collectablePersistenceObject[i].transform.parent.CompareTag("Prototype")){
                Destroy(_collectablePersistenceObject[i].gameObject);
                Debug.Log("Destroyed collectable " + _collectablePersistenceObject[i].name);
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
            obj.transform.name = Settings.gameData.collectableItemsPrefabs[i];
        }
        
        GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG).GetComponent<FPSInput>().SetSaveData();
    }
    private string GetPrefabName(string name){
        string prefabName = name;    
        if(prefabName.Contains("key", System.StringComparison.OrdinalIgnoreCase)){
            prefabName = "Key";
        }
        return prefabName;
    }

    public GameObject CreateInventoryItem(string objectName){
        string prefabName = GetPrefabName(objectName);
        GameObject objPrefab = _collectablePrototypes.transform.Find(prefabName).gameObject;
        GameObject obj = Instantiate(objPrefab);
        obj.name = objectName;
        Debug.Log("Created inventory item in scene " + obj.name);

        return obj;
    }
    private List<IDataPersistenceSave> FindAllDataPersistenceObjects(){
        IEnumerable<IDataPersistenceSave> data = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistenceSave>();
        return new List<IDataPersistenceSave>(data);
    }
    private List<Collectable> FindAllCollectableObjects(){
        IEnumerable<Collectable> data = FindObjectsOfType<MonoBehaviour>().OfType<Collectable>();
        return new List<Collectable>(data);
    }
}
