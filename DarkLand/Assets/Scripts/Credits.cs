using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] private GameObject button;
    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(false);
    }



    public void ActivateButton(){
        button.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
