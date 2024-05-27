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

    public void AddItem(GameObject obj, int usages){
        obj.GetComponent<Collectable>().enabled = false;
        obj.GetComponent<Collider>().enabled = false;
        Rigidbody rb ;

        if(obj.TryGetComponent<Rigidbody>(out rb))
            rb.isKinematic = true;
        obj.GetComponent<InteractableTrigger>().enabled = false;
        if(obj == null)
            return;
        string name = obj.name;
        
        if(!_items.ContainsKey(name)){
            _items.Add(name, usages);

            IUsableObject usable;
            if(obj.TryGetComponent<IUsableObject>(out usable)){
                Managers.UsableInventory.AddSelectable(obj);
            }

        }
        else 
            _items[name]+=usages;
        
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
