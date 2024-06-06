using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using UnityEngine;



public class ResourceLoader{
    private static Texture2D cursor;
    private static Dictionary<string, Sprite> sprites = new Dictionary<string,Sprite>();
     private static Dictionary<string, string> descriptions = new Dictionary<string, string>();

    private static Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private const string path = "descriptions";
    private static string cursorPath = "Cursor";
    public static Sprite GetImage(string name){
        Debug.Log("Image: "+name);
        if(!sprites.ContainsKey(name)){
             sprites.Add(name, Resources.Load<Sprite>(Path.Combine(Settings.INVENTORY_SPRITES_FOLDER_NAME, name)));
        }
        return sprites[name];

    }

    public static AudioClip GetSound(string sound){
            if(!audioClips.ContainsKey(sound)){
             audioClips.Add(sound, Resources.Load<AudioClip>(Path.Combine(Settings.SOUND_FOLDER_NAME, sound )));
        }
        return audioClips[sound];
    }


    public static Texture2D GetCursor(){
        if(cursor == null){
            cursor = Resources.Load<Texture2D>(cursorPath);
        }
        return cursor;
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
