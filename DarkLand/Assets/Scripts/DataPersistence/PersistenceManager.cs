using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PersistenceManager : MonoBehaviour, IGameManager{
    public ManagerStatus status {get;private set;}

    private GameData gameData;
    [SerializeField] private string _fileName;
    private List<IDataPersistenceSave> _dataPersistenceObjects;
    private FileDataHandler _dataHandler;
    public void Startup(){
        
    }

    public void NewGame(){
        Debug.Log("New game creation");
    }

    public void SaveGame(){
        gameData = new GameData();
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach(IDataPersistenceSave dataPersObj in _dataPersistenceObjects){
           dataPersObj?.SaveData(ref gameData); 
        }
        FileDataHandler.Save(gameData, _fileName);
        Debug.Log("Saved data");
    }

    private List<IDataPersistenceSave> FindAllDataPersistenceObjects(){
        IEnumerable<IDataPersistenceSave> data = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistenceSave>();
        return new List<IDataPersistenceSave>(data);
    }
}
