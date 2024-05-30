using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader{

    public static void Load(string dataFileName){
        Settings.gameData = FileDataHandler.Load(dataFileName);
    }
}
