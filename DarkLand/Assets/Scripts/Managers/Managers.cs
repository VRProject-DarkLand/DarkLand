using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
public class Managers : MonoBehaviour
{
    public static PlayerManager Player {get; private set;}
    public static InventoryManager Inventory {get; private set;}
    public static UsableObjectManager UsableInventory {get; private set;}
    public static AudioManager AudioManager {get; private set;}
    public static PauseManager Pause {get; private set;}
    public static PersistenceManager Persistence {get; private set;}
    public static PointerManager PointerManager {get; private set;}
    public static DropsManager DropsManager {get; private set;}
    // Start is called before the first frame update
    private List<IGameManager> _startSequence;
    void Start()
    {
        
    }

    void Awake(){
        //Settings.LoadData;
        //LoadManager.Load();
        Time.timeScale = 1f;
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        UsableInventory = GetComponent<UsableObjectManager>(); 
        Pause = GetComponent<PauseManager>();
        Persistence = GetComponent<PersistenceManager>();
        AudioManager = GetComponent<AudioManager>();
        PointerManager = GetComponent<PointerManager>();
        DropsManager = GetComponent<DropsManager>();
        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(PointerManager);
        _startSequence.Add(AudioManager);
        _startSequence.Add(Inventory);
        _startSequence.Add(UsableInventory);
        _startSequence.Add(Pause);
        _startSequence.Add(Persistence);
        _startSequence.Add(DropsManager);
        
        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers(){
        foreach (IGameManager manager in _startSequence) {
            manager?.Startup();
        }
        yield return null;
        int numModules = _startSequence.Count;
        int numReady = 0;
        while (numReady < numModules) {
            int lastReady = numReady;
            numReady = 0;
            foreach (IGameManager manager in _startSequence) {
                if(manager == null){
                    numModules-=1;
                    continue;
                }
                if (manager.status == ManagerStatus.Started) {
                    numReady++;
                }
            }
            if (numReady > lastReady) {
                Debug.Log ("Progress: " + numReady + "/" + numModules);
            }
            yield return null;
        }
        Debug.Log("All managers started up");
        //once all managers have started up it is possible to set the data of
        //a saving that has been loaded. This is done only if the game is loading a save
        if(Settings.LoadedFromSave){
            //create monsters and collectables
            if(Persistence != null)
                Persistence.SetLoadedData();
            //set stats to player
            if(Player != null)
                Player.SetLoadGameData();
            //create inventory from snapshot
            if(Inventory != null)
                Inventory.SetLoadedGameData();
        } 
        Time.timeScale = 1f;
        Messenger.Broadcast(GameEvent.ALL_MANAGERS_LOADED,  MessengerMode.DONT_REQUIRE_LISTENER);
        
    }
    public static void LoadGameData(){

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
