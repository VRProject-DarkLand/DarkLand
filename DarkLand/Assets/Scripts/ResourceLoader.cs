using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using UnityEngine;



public class ResourceLoader{
    private static Dictionary<string, Sprite> sprites = new Dictionary<string,Sprite>();
     private static Dictionary<string, string> descriptions = new Dictionary<string, string>();
    private const string path = "descriptions";
    public static Sprite GetImage(string name){
        if(name.EndsWith("key", System.StringComparison.OrdinalIgnoreCase)){
            name = "Key";
        }
        if(!sprites.ContainsKey(name)){
             sprites.Add(name, Resources.Load<Sprite>(Path.Combine(Settings.INVENTORY_SPRITES_FOLDER_NAME, name)));
        }
        return sprites[name];

    }

    public static string GetDescription(string name){
        if(!descriptions.ContainsKey(name)){
            try{
                string description = Resources.Load<TextAsset>(Path.Combine(Settings.INVENTORY_ITEMS_DESCRIPTIONS_FOLDER_NAME, name)).text;
                if(description!= null)
                    descriptions.Add(name, description);  
            }catch(Exception e){
                descriptions.Add(name, "It seems like a useful item");
            }
        }  
        return descriptions[name];

    }
}
