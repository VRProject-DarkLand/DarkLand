using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI id;
    [SerializeField] private TextMeshProUGUI profile;
    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI sceneNameGUI;
    private static  Color defColor = new Color32(176,1,1,160);
    private static Color selected = new Color32(255,255,255,160);
    public string fileName {get;private set;} = "";
    public string sceneName = Settings.ASYLUM_SCENE;
    // Start is called before the first frame update
    void Start()
    {
        Deselect();
    }

    public void SetInfo(string id, string filename, string time){
        this.fileName = filename;
        string [] elements = fileName.Split('_');
        this.id.text = id;
        this.sceneName =  elements[0];
        this.sceneNameGUI.text = this.sceneName;
        this.profile.text =  elements[1];
        this.time.text = time;
    }

    public void Select(){
        GetComponent<Image>().color = selected;
        foreach(TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>()){
            text.color = Color.black;
        }
    }

    public void Deselect(){
        GetComponent<Image>().color = defColor;
        foreach(TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>()){
            text.color = Color.white;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
