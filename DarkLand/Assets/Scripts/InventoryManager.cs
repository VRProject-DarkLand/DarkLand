using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    // Start is called before the first frame update


    public ManagerStatus status {get;private set;}
    private  Dictionary<string, int> _items; 

    public void Startup(){
        Debug.Log("Inventory manager starting...");
        status = ManagerStatus.Started;
        _items = new Dictionary<string, int>();
    }
    void Start()
    {
        
    }

    private void DisplayItems(){
        string itemDisplay = "List of Items ";
        foreach(var item in _items){
            itemDisplay += item.Key +": "+item.Value+" ";
        }
        Debug.Log(itemDisplay);
    }

    public void AddItem(GameObject obj){
        obj.GetComponent<Collectable>().enabled = false;
        if(obj == null)
            return;
        string name = obj.name;
        Managers.UsableInventory.AddSelectable(obj);
        if(!_items.ContainsKey(name))
            _items.Add(name, 1);
        else 
            _items[name]+=1;
        DisplayItems();
    }

    public List<string> GetItemList(){
        List<string> list = new List<string>(_items.Keys);
        return list;
    }

    public int GetItemCount(string name){
        if(_items.ContainsKey(name))
            return _items[name];
        return 0;
    }

    public void ConsumeItem(string name){
        if(_items.ContainsKey(name)){
            _items[name] -= 1;
            if(_items[name] == 0){
                _items.Remove(name);    
            }
        }
        else{
            Debug.Log("Cannot consume item");
        }
        DisplayItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
