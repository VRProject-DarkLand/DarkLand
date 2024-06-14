using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class keeps track of the data of all the persistence 
// data objects and is used to serialize/deserialize data
[System.Serializable]
public class GameData{
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public int playerHealth;
    
    public List<string> collectableItemsPrefabs;
    public List<string> collectableItemsNames;
    public List<Vector3> collectableItemsPosition;
    public List<Vector3> collectableItemsRotation;
    public List<Vector3> collectableItemsScale;
    //elements are pushed slot by slot and therefore
    // just the name suffices to reconstruct the order
    public List<string> usableItemsNames;

    public List<string> inventoryItemsNames;
    public List<string> inventoryItemsPrefabs;
    public List<string> inventoryItemsQuantities;

    public List<ScaryGirlAI.ScaryGirlSavingData> scaryGirlsData;
    public List<LittleGirlAI.LittleGirlSavingData> littleGirlsData;

    public List<WaypointMover.SpiderData> spidersData;
    public bool allLightsStatus = true;

    public List<SlidingCrate.WeaponBoxData> weaponBoxes;
    public GameData(){
        collectableItemsPrefabs = new List<string>();
        collectableItemsNames = new List<string>();
        collectableItemsPosition = new List<Vector3>();
        collectableItemsRotation = new List<Vector3>();
        collectableItemsScale = new List<Vector3>();
        usableItemsNames = new List<string>();
        inventoryItemsNames = new List<string>();
        inventoryItemsQuantities = new List<string>();
        scaryGirlsData = new List<ScaryGirlAI.ScaryGirlSavingData>();
        littleGirlsData = new List<LittleGirlAI.LittleGirlSavingData>();
        spidersData = new List<WaypointMover.SpiderData>();
        inventoryItemsPrefabs = new List<string>();
        weaponBoxes = new List<SlidingCrate.WeaponBoxData>();
    }

}
