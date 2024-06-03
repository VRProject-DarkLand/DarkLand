using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager, IDataPersistenceSave
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

    public bool AddItem(GameObject obj, int usages){
        Collectable collectable = obj.GetComponent<Collectable>();
        collectable.enabled = false;
        obj.GetComponent<Collider>().enabled = false;
        Rigidbody rb ;

        if(obj == null){
            return false;
        }
        if(obj.TryGetComponent<Rigidbody>(out rb))
            rb.isKinematic = true;
        obj.GetComponent<InteractableTrigger>().enabled = false;
        string name = obj.name;
        
        if(!_items.ContainsKey(name)){
            _items.Add(name, new InventoryItem(name, usages));

            IUsableObject usable;
            if(obj.TryGetComponent<IUsableObject>(out usable)){
                Managers.UsableInventory.AddSelectable(obj);
            }

        }
        else 
            _items[name].Increase(usages);
        
        DisplayItems();
        return true;
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
    public void SetLoadedGameData(){
        //instantiate object, then collect
        for(int i = 0; i < Settings.gameData.inventoryItemsNames.Count; ++i){
            GameObject obj = Managers.Persistence.CreateInventoryItem(Settings.gameData.inventoryItemsNames[i]);
            AddItem(obj, obj.GetComponent<Collectable>().GetMaxUsages());
            //Collectable coll = obj.GetComponent<Collectable>();
            Debug.Log("Destroy inv obj " + obj.name);
            Destroy(obj);
        }
    }
    public void SaveData(){
        List<InventoryItem> items = GetItems();
        foreach(InventoryItem item in items){
            Settings.gameData.inventoryItemsNames.Add(item.Name);
            Settings.gameData.inventoryItemsQuantities.Add(item.Count.ToString());
        }
    }
}

public class InventoryItem{
    public string Name {get; private set;}
    public int Count {get; private set;}

    public InventoryItem(string name, int count ){
        Name = name;
        Count = count; 
    }

    public void Decrease(){
        Count-=1;
    }
    public void Increase(int times){
        Count+=times;
    }

    
}
