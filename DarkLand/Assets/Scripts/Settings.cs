using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public const string TORCH_TAG = "Torch";
    public const string PLAYER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";
    public static string LastSaving = "";
    public static bool LoadedFromSave = false;
    public const string ASYLUM_SCENE = "Scenes/Asylum";
    public const string MAIN_MENU = "Scenes/MainMenu";
    public static GameData gameData;
    public static string SAVE_DIR = Application.persistentDataPath;
    public const string HEALTH = "Adrenaline";
    public const string INVENTORY_SPRITES_FOLDER_NAME = "InventoryIcons";
    public const string INVENTORY_ITEMS_DESCRIPTIONS_FOLDER_NAME = "InventoryDescriptions";
    public static class AudioSettings{
        public static float musicVolume = 0.0f;
        public static float soundVolume = 1f;
        public static bool musicOn = true;
        public static bool soundOn = true;
    }
}
