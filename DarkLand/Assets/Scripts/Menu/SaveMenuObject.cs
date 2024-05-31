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
    private static  Color defColor = new Color32(176,1,1,160);
    private static Color selected = new Color32(255,255,255,160);
    public string fileName {get;private set;} = "";
    // Start is called before the first frame update
    void Start()
    {
        Deselect();
    }

    public void SetInfo(string id, string filename){
        this.fileName = filename;
        string [] elements = fileName.Split('_');
        this.id.text = id;
        this.profile.text =  elements[0];
        this.time.text =  elements[1] +" "+ elements[2]+":"+ elements[3];
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
