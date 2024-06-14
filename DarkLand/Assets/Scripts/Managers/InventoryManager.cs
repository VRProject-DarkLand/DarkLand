using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager, IDataPersistenceSave
{


    public ManagerStatus status {get;private set;}
    private  Dictionary<string, InventoryItem> _items; 
    public void Startup(){
        status = ManagerStatus.Started;
        _items = new Dictionary<string, InventoryItem>();
    }
    void Start()
    {
        
    }

    private void DisplayItems(){
    }
    //adds an item to the inventory, and if the item is also usable it is also added
    // to the usable inventory 
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
            _items.Add(name, new InventoryItem(collectable.InventoryName, usages));

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

    public List<string> GetItemsKey(){
        return _items.Keys.ToList<string>();
    }

    public InventoryItem GetItem(string name){
        if(_items.ContainsKey(name))
            return _items[name];
        return null;
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
            GameObject obj = Managers.Persistence.CreateInventoryItem(Settings.gameData.inventoryItemsPrefabs[i]);
            obj.name = Settings.gameData.inventoryItemsNames[i];
            int quantity ;
            if(!int.TryParse(Settings.gameData.inventoryItemsQuantities[i], out quantity)){
                quantity = obj.GetComponent<Collectable>().GetMaxUsages();
            }
            AddItem(obj, quantity);
            //Collectable coll = obj.GetComponent<Collectable>();
            //Debug.Log("Destroy inv obj " + obj.name);
            Destroy(obj);
        }
    }
    public void SaveData(){
        List<string> items = GetItemsKey();
        foreach(string item in items){
            Settings.gameData.inventoryItemsNames.Add(item);
            Settings.gameData.inventoryItemsPrefabs.Add(_items[item].InventoryClassKey);
            Settings.gameData.inventoryItemsQuantities.Add(_items[item].Count.ToString());
        }
    }
}

public class InventoryItem{
    public string InventoryClassKey {get; private set;}
    public int Count {get; private set;}

    public InventoryItem(string name, int count ){
        InventoryClassKey = name;
        Count = count; 
    }

    public void Decrease(){
        Count-=1;
    }
    public void Increase(int times){
        Count+=times;
    }

    
}
