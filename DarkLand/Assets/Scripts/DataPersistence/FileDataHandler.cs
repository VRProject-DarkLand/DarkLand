using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileDataHandler{
    public static void Load(string dataFileName){
        string fullPath = Path.Combine(Settings.SAVE_DIR, dataFileName);
        Settings.gameData = new GameData();
        if(File.Exists(fullPath)){
            try{
                string loadedDataString = "";            
                using(StreamReader sr = new(fullPath)){
                    loadedDataString = sr.ReadToEnd();
                }
                // serialize string data to serializable GameData
                Settings.gameData = JsonUtility.FromJson<GameData>(loadedDataString);
            }catch(Exception e){
                Debug.Log("Error while loading saving data " + e);
            }
        }else{
            Debug.Log("File does not exist in load");
        }
    }
    //save current game data to file
    public static void Save(string dataFileName){
        string fullPath = Path.Combine(Settings.SAVE_DIR, dataFileName);
        try{
            //create a parent directory in which the savings fil will be put
            //Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //serialize the content of the data in JSON format
            File.WriteAllText(fullPath, string.Empty);
            string dataString = JsonUtility.ToJson(Settings.gameData, true);
            
            StreamWriter streamWriter = new StreamWriter(fullPath, false);
            streamWriter.Write(dataString);
            streamWriter.Close();

        }catch(Exception e){
           Debug.LogError("Error while trying to save in " + fullPath + " "+ e); 
        }
    }

}
