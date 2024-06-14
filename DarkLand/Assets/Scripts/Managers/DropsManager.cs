using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsManager : MonoBehaviour, IGameManager{

    public ManagerStatus status {get; private set;}
    [SerializeField] public GameObject [] items;
    [SerializeField] public GameObject allCollectables;
    public void Startup(){
       status = ManagerStatus.Started;
    }
    //drop a random item between the available ones
    //called when a sliding crate is opened by the player    
    public GameObject DropRandomItem(){
        int index = Random.Range(0, items.Length);
        GameObject drop = Instantiate(items[index]);
        drop.name = items[index].name;
        drop.transform.SetParent(allCollectables.transform);
        return drop;
    }
}
