using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistenceSave{
    public void SaveData(ref GameData data);
}
