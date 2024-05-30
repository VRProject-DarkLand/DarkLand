using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileDataHandler{
    public static GameData Load(string dataFileName){
        string fullPath = Path.Combine(Settings.SAVE_DIR, dataFileName);
        Debug.Log("Loading game data from: " + fullPath);
        GameData loadedData = null;
        if(File.Exists(fullPath)){
            try{
                string loadedDataString = "";            
                using(StreamReader sr = new(fullPath)){
                    loadedDataString = sr.ReadToEnd();
                    Debug.Log("Correctly loaded");
                }

                // serialize string data to serializable GameData
                Debug.Log("Read data: " + loadedDataString);
                loadedData = JsonUtility.FromJson<GameData>(loadedDataString);
            }catch(Exception e){
                Debug.Log("Error while loading saving data " + e);
            }
        }else{
            Debug.Log("File does not exist in load");
        }
        return loadedData;
    }
    //save current game data to file
    public static void Save(GameData data, string dataFileName){
        string fullPath = Path.Combine(Settings.SAVE_DIR, dataFileName);
        try{
            //create a parent directory in which the savings fil will be put
            Debug.Log("Creating savings file  " + fullPath);
            //Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //serialize the content of the data in JSON format
            string dataString = JsonUtility.ToJson(data, true);
            // using (FileStream stream = new FileStream(fullPath, FileMode.OpenOrCreate)){
            //     using (StreamWriter writer = new StreamWriter(fullPath, false)){
            //         writer.Write(dataString);
            //         //writer.Close();
            //     }
            // }
            
            FileStream fileStream = new FileStream(fullPath, 
                                       FileMode.OpenOrCreate, 
                                       FileAccess.ReadWrite, 
                                       FileShare.None);
            fileStream.Write(Encoding.UTF8.GetBytes(dataString));
            fileStream.Close();

        }catch(Exception e){
           Debug.LogError("Error while trying to save in " + fullPath + " "+ e); 
        }
    }

}
