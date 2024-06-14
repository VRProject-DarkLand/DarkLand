using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//implemented by persistent data object - each object has
// the resposnibility of adding its data do GameData
public interface IDataPersistenceSave{
    public void SaveData();
}
