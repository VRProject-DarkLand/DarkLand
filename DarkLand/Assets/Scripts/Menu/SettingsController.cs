using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
     [SerializeField] private GameObject settings;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDrop;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vsynch;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider sensibilitySlider;
    [SerializeField] private Slider aimingSlider;
    [SerializeField] private TextMeshProUGUI musicNumber;
    [SerializeField] private TextMeshProUGUI soundNumber;
    [SerializeField] private TextMeshProUGUI sensibilityNumber;
    [SerializeField] private TextMeshProUGUI aimingNumber;
    private List<GameObject> panels ;

    private Resolution[] resolutions; 

    void Awake(){
       
        musicNumber.text = Mathf.RoundToInt(musicSlider.value*100).ToString();
        soundNumber.text = Mathf.RoundToInt(soundSlider.value*100).ToString();
        sensibilitySlider.value = Mathf.Round(Settings.Sensibility*Settings.MinSensibility/Settings.MaxSensibility);
        aimingSlider.value = Settings.AimSensibility*Settings.MinSensibility/Settings.MaxSensibility;
        sensibilityNumber.text = Settings.Sensibility.ToString();
        aimingNumber.text = Settings.AimSensibility.ToString("F1");
       
    }
    // Start is called before the first frame update
    void Start()
    {
        panels = new List<GameObject>(){audioPanel, infoPanel,videoPanel};
        qualityDrop.value = QualitySettings.GetQualityLevel();
        qualityDrop.RefreshShownValue();
        vsynch.isOn = QualitySettings.vSyncCount > 0;
        
        Screen.fullScreen = true;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resOpt = new List<string>();
        int currentResolution = 0;
        for (int i = 0; i< resolutions.Length;++i){
            Resolution resolution = resolutions[i];
            string opt = resolution.width +" x "+resolution.height;
            resOpt.Add(opt);
            if(resolution.width == Screen.width && resolution.height == Screen.height){
                currentResolution = i;
            }
        }
        musicToggle.isOn = Managers.AudioManager.musicOn;
        soundToggle.isOn = Managers.AudioManager.AllSoundOn;
        soundSlider.value = Managers.AudioManager.soundVolume;
        musicSlider.value = Managers.AudioManager.musicVolume;
        resolutionDropdown.AddOptions(resOpt);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue(); 
    }

    public void SetResolution(int  resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int indQuality){
        QualitySettings.SetQualityLevel(indQuality);
    }

    public void SetVSynch(bool vSynch){
        if(vSynch){
            QualitySettings.vSyncCount = 2;
        }else
            QualitySettings.vSyncCount = 0;
    }

    public void HideAll(){
        foreach(GameObject g in panels){
            g.SetActive(false);
        }
    }

    public void ActivePanel(bool active){
        settings.SetActive(active);
    }

    public void OnMusicChange(float value){
         musicNumber.text = Mathf.RoundToInt(musicSlider.value*100).ToString();
    }

    public void OnSoundChange(float value){
         soundNumber.text = Mathf.RoundToInt(musicSlider.value*100).ToString();
    }


    public void OnSensibilityChange(float value){
        Settings.Sensibility = Mathf.Round(Mathf.Lerp(Settings.MinSensibility, Settings.MaxSensibility, value));
        sensibilityNumber.text = Settings.Sensibility.ToString();
        Messenger.Broadcast(GameEvent.SENSIBILITY_CHANGE, MessengerMode.DONT_REQUIRE_LISTENER);
    }

    public void OnAimingSensibilityChange(float value){
        Settings.AimSensibility = Mathf.Lerp(Settings.MinSensibility, Settings.MaxSensibility, value);
        aimingNumber.text = Settings.AimSensibility.ToString("F1");
        Messenger.Broadcast(GameEvent.SENSIBILITY_CHANGE, MessengerMode.DONT_REQUIRE_LISTENER);
    }

    public void OnPanelShow(GameObject go){
        HideAll();
        go.SetActive(true);
    }
    // public void SetVSynch(bool vSynch){
    //     if (vSynch){
    //         QualitySettings.vSyncCount = Settings.VSYNCH;    
    //     }else{
    //         QualitySettings.vSyncCount = 0;
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
