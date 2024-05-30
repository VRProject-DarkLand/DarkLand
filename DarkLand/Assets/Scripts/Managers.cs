using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))]
public class Managers : MonoBehaviour
{
    public static PlayerManager Player {get; private set;}
    public static InventoryManager Inventory {get; private set;}
    public static UsableObjectManager UsableInventory {get; private set;}
    public static PauseManager Pause {get; private set;}
    public static PersistenceManager Persistence {get; private set;}

    // Start is called before the first frame update
    private List<IGameManager> _startSequence;
    void Start()
    {
        
    }

    void Awake(){
        //Settings.LoadData;
        //LoadManager.Load();
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        UsableInventory = GetComponent<UsableObjectManager>(); 
        Pause = GetComponent<PauseManager>();
        Persistence = GetComponent<PersistenceManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);
        _startSequence.Add(UsableInventory);
        _startSequence.Add(Pause);
        
        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers(){
        foreach (IGameManager manager in _startSequence) {
            manager.Startup();
        }
        yield return null;
        int numModules = _startSequence.Count;
        int numReady = 0;
        while (numReady < numModules) {
            int lastReady = numReady;
            numReady = 0;
            foreach (IGameManager manager in _startSequence) {
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
        
    }
    public static void LoadGameData(){

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
