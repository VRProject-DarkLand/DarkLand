using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISound : MonoBehaviour
{
    [SerializeField] AudioClip click;
    //[SerializeField] AudioClip hover;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick(){
        Managers.AudioManager.PlaySound(click);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
