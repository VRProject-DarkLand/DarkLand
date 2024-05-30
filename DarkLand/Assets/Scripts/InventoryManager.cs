using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    // Start is called before the first frame update


    public ManagerStatus status {get;private set;}
    private  Dictionary<string, InventoryItem> _items; 
    public void Startup(){
        Debug.Log("Inventory manager starting...");
        status = ManagerStatus.Started;
        _items = new Dictionary<string, InventoryItem>();
    }
    void Start()
    {
        
    }

    private void DisplayItems(){
    }

    public void AddItem(GameObject obj, int usages){
        obj.GetComponent<Collectable>().enabled = false;
        obj.GetComponent<Collider>().enabled = false;
        Rigidbody rb ;

        if(obj == null)
            return;
        if(obj.TryGetComponent<Rigidbody>(out rb))
            rb.isKinematic = true;
        obj.GetComponent<InteractableTrigger>().enabled = false;
        string name = obj.name;
        
        if(!_items.ContainsKey(name)){
            _items.Add(name, new InventoryItem(name, "", usages));

            IUsableObject usable;
            if(obj.TryGetComponent<IUsableObject>(out usable)){
                Managers.UsableInventory.AddSelectable(obj);
            }

        }
        else 
            _items[name].Increase(usages);
        
        DisplayItems();
    }

    public List<string> GetItemList(){
        List<string> list = new List<string>(_items.Keys);
        return list;
    }

    public void ChangeInventoryVisibility(){
        Messenger<bool>.Broadcast(GameEvent.SHOW_INVENTORY, GameEvent.isInventoryOpen);
    }

    public int GetItemCount(string name){
        if(_items.ContainsKey(name))
            return _items[name].Count;
        return 0;
    }

    public List<InventoryItem> GetItems(){
        return _items.Values.ToList<InventoryItem>();
    }

    public void ConsumeItem(string name){
        if(_items.ContainsKey(name)){
            _items[name].Decrease();
            if(_items[name].Count == 0){
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

public class InventoryItem{
    public string Name {get; private set;}
    public string Description {get; private set;}
    public int Count {get; private set;}

    public InventoryItem(string name, string description, int count ){
        Name = name;
        Description = description;
        Count = count; 
    }

    public void Decrease(){
        Count-=1;
    }
    public void Increase(int times){
        Count+=times;
    }

    
}
