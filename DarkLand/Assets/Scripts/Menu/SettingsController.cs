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
    private List<GameObject> panels ;

    private Resolution[] resolutions; 

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
