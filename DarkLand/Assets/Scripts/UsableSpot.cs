using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsableSpot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image item;
    [SerializeField] private Image background;
    [SerializeField] private Image selected;
    void Start()
    {
        SetItem(null);
        Deselect();

    }

    public void Select(){
        selected.enabled = true;
    }
    public void Deselect(){
        selected.enabled = false;
    }

    public void SetItem(Sprite sprite){
        if(sprite == null){
            item.enabled = false;
            return;            
        }
        item.sprite = sprite;
        item.enabled = true;

    }

    // Update is called once per frame
}
