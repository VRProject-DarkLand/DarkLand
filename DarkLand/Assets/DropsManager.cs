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

    public GameObject DropRandomItem(){
        Debug.Log("Called drop");
        int index = Random.Range(0, items.Length);
        GameObject drop = Instantiate(items[index]);
        drop.name = items[index].name;
        drop.transform.SetParent(allCollectables.transform);
        return drop;
    }
}
