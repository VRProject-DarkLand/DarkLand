using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData{

    public Vector3 playerPosition;
    public Vector3 playerRotation;
    // public Vector3 playerRotation {get; set;}
    public int playerHealth;
    
    public List<string> collectableItemsPrefabs;
    public List<Vector3> collectableItemsPosition;
    public List<Vector3> collectableItemsRotation;
    public List<Vector3> collectableItemsScale;
    //elements are pushed slot by slot and therefore
    // just the name suffices to reconstruct the order
    public List<string> usableItemsNames;

    public List<string> inventoryItemsNames;
    public List<string> inventoryItemsQuantities;

    public GameData(){
        collectableItemsPrefabs = new List<string>();
        collectableItemsPosition = new List<Vector3>();
        collectableItemsRotation = new List<Vector3>();
        collectableItemsScale = new List<Vector3>();
        usableItemsNames = new List<string>();
        inventoryItemsNames = new List<string>();
        inventoryItemsQuantities = new List<string>();
    }

}
