using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySpriteLoader
{
    // Start is called before the first frame update
    private static Dictionary<string, Sprite> sprites = new Dictionary<string,Sprite>();

    public static Sprite GetImage(string name){
        if(name.EndsWith("key", System.StringComparison.OrdinalIgnoreCase)){
            name = "key";
        }
        if(!sprites.ContainsKey(name)){
             sprites.Add(name, Resources.Load<Sprite>("InventoryIcons/"+name));
        }
        return sprites[name];

    }
}
