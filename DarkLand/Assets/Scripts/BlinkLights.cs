using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkLights : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject lightGameObject;
    private GameObject attachedPointLight;
    private Material emissiveMaterial;
    private Color originalEmissiveColor;

    void Start(){
        lightGameObject = gameObject;
        attachedPointLight = lightGameObject.transform.GetChild(0).gameObject;
        emissiveMaterial = GetComponent<Renderer>().material;
        originalEmissiveColor = emissiveMaterial.GetColor("_EmissionColor");
        StartCoroutine(BlinkManager());
    }

    // Update is called once per frame
    void Update(){
        
    }
    IEnumerator BlinkManager(){
        while(true){
            StartCoroutine(Blink());
            yield return new WaitForSeconds(9f);
        }
    }
    IEnumerator Blink(){
        attachedPointLight.GetComponent<Light>().enabled = false;
        emissiveMaterial.SetColor("_EmissionColor", Color.black);
        yield return new WaitForSeconds(2f);
        attachedPointLight.GetComponent<Light>().enabled = true;
        emissiveMaterial.SetColor("_EmissionColor", originalEmissiveColor);
        yield return new WaitForSeconds(2f);
        
    }
}
