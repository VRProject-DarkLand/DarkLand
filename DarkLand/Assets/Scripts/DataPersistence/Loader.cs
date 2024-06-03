using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader{

    public static void Load(string dataFileName){
        FileDataHandler.Load(dataFileName);
        Settings.LoadedFromSave = true;
    }
}
