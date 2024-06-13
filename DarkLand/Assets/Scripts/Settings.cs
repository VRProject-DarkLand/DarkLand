using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public const string TORCH_TAG = "Torch";
    public const string PLAYER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";

    public const string INTERACTION_ENEMY_TAG = "InteractiveEnemy";
    public static string LastSaving = "";
    public static bool LoadedFromSave = false;
    public static bool canSave = true;
    public const string ASYLUM_NAME = "Asylum";
    public const string ASYLUM_SCENE = "Scenes/Asylum";
    public const string MAIN_MENU = "Scenes/MainMenu";
    public const string OUTSIDE_SCENE = "Scenes/Forest";
    public static GameData gameData;
    public static bool GameFinished = false;
    public static string SAVE_DIR = Application.persistentDataPath;
    public const string HEALTH = "Adrenaline";
    public const string AMMO_NAME = "Ammo Box";

    public static float MinSensibility = 1f;
    public static float MaxSensibility = 15f; 
    public static float AimSensibility = 4.5f; 
    public static float Sensibility = 9.0f;

    public static float DEFAULT_FOV = 65;
    public static float AIM_FOV = 40;

    public const string SOUND_FOLDER_NAME = "Sound";
    public const string INVENTORY_SPRITES_FOLDER_NAME = "InventoryIcons";
    public const string INVENTORY_ITEMS_DESCRIPTIONS_FOLDER_NAME = "InventoryDescriptions";
    public static int RAYCAST_MASK = ~LayerMask.GetMask("Ignore Raycast");
    public static class AudioSettings{
        public static float musicVolume = 0.7f;
        public static float soundVolume = 1f;
        public static bool musicOn = true;
        public static bool soundOn = true;
        public static string SLIDING_CRATE = "boxOpen";
        public const string SPIDER_SOUND = "spiderSound";
        public const string SPIDER_HURT_SOUND = "spiderHurt";
        public const string GUN_SHOOT = "gun-shots-pistol-1";
        public const string NO_AMMO = "noAmmo";
        public const string DOOR_OPEN_SOUND = "DoorOpensSound";
        public const string DOOR_CLOSE_SOUND = "DoorClosesSound";
    }
}
