using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryDescriptionLoader 
{
    private static Dictionary<string, string> descriptions = null;
    private const string path = "descriptions";
    public static string GetDescription(string name){
        if(descriptions == null){
           TextAsset t = Resources.Load<TextAsset>(path);
           string text = t.text;
           Debug.Log(text);
           descriptions = JsonUtility.FromJson<Dictionary<string,string>>(t.text);
        }
        if(!descriptions.ContainsKey(name))
            return "It seems to be a useful object...";   
        return descriptions[name];

    }
}
