using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightsManager : MonoBehaviour
{
    private  GameObject player;
    private  List<Light> allLights;

    public bool on {get;private set;} = true;


    void Start(){
        Messenger<bool>.AddListener(GameEvent.OPERATE_ON_LIGHTS, OperateOnLights);
        Messenger.AddListener(GameEvent.ALL_MANAGERS_LOADED, ActivatePointLightManager); 
    }
    public void ActivatePointLightManager(){
        // Find the player GameObject (assuming it has a tag "Player")
        player = GameObject.FindGameObjectWithTag("Player");
        // Find all GameObjects with Light component
        allLights = FindObjectsOfType<Light>().ToList();
        for(int i= 0;i<allLights.Count;++i){
            if (allLights[i].gameObject.tag == Settings.TORCH_TAG){
                    allLights.Remove(allLights[i]);
            }
        }
        StartCoroutine(BlinkManager());
    }
    //make the a light near to the player to blink with a certain frequency
    IEnumerator Blink(GameObject selectedLight){
        GameObject parent = selectedLight.transform.parent.gameObject;
        Material emissiveMaterial = parent.GetComponent<Renderer>().material;
        Color originalEmissiveColor = emissiveMaterial.GetColor("_EmissionColor");  
        Light l = selectedLight.GetComponent<Light>();
        for(int i = 0; i< Random.Range(1,4);++i){
            l.enabled = false;
            emissiveMaterial.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.8f));
            l.enabled = true;
            emissiveMaterial.SetColor("_EmissionColor", originalEmissiveColor);
            
            l.intensity = 1.7f;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
            l.enabled = false;
            emissiveMaterial.SetColor("_EmissionColor", Color.black);
            l.intensity = 1;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.8f));
            emissiveMaterial.SetColor("_EmissionColor", originalEmissiveColor);
            l.enabled = on;
        }
    }
    //start the blink manager, which actually exectues during the entire
    //lifetime of the scene and that will call the blink coroutine
    IEnumerator BlinkManager(){
        while(true){
            while(!on){
                yield return new WaitForSeconds(10);
            }
            if(allLights.Count <=1)
                yield break;
            // Create a list to store the point lights and their distances
            List<KeyValuePair<Light, float>> lightDistances = new List<KeyValuePair<Light, float>>();
            // Calculate distances and store them along with the lights
            foreach (Light light in allLights)
            {
                if(light == null){
                    Debug.Log("Null light");
                    continue;
                }
                if (light.type == LightType.Point)
                {
                    if(light.transform.position.y > player.transform.position.y-1){
                        Vector3 directionToTarget = player.transform.position - light.transform.position;
                        float angle = Vector3.Angle(player.transform.forward, directionToTarget);
                        //float distance = Vector3.Distance(light.transform.position, player.transform.position);
                        float distance = directionToTarget.magnitude;
                         if (Mathf.Abs(angle) < 180)
                            lightDistances.Add(new KeyValuePair<Light, float>(light, distance));
                    }
                }
            }


            // Sort the list based on distances
            lightDistances.Sort((x, y) => x.Value.CompareTo(y.Value));

            // Output the sorted list
            int randomIndex = Random.Range(0, Mathf.Min(lightDistances.Count, 4));
            Light selectedLight = lightDistances[randomIndex].Key;
            StartCoroutine(Blink(selectedLight.gameObject));
            yield return new WaitForSeconds(Random.Range(3f, 12f));
        }
    }
    //switch status of all the lights inside the asylum
    //called by spiders when they die
    private void OperateOnLights(bool turnOn){
        if(allLights == null)
            ActivatePointLightManager();
        on = turnOn;
        Color emissionColor = Color.black;
        if(turnOn){
          emissionColor = Color.white;
        } 
          foreach (Light light in allLights)
            {
                if(light == null)
                    continue;
                Material emissiveMaterial = null;
                Renderer r;
                if(light.transform.parent.gameObject.TryGetComponent<Renderer>(out r)){
                    emissiveMaterial = r.material;
                }else if( light.transform.gameObject.TryGetComponent<Renderer>(out r)){
                    emissiveMaterial = r.material;
                }
                if(emissiveMaterial == null){
                    continue;
                }
                //Color originalEmissiveColor = emissiveMaterial.GetColor("_EmissionColor");  
                Light l = light.GetComponent<Light>();
                l.enabled = turnOn;
                emissiveMaterial.SetColor("_EmissionColor", emissionColor);
            }
    }

    void OnDestroy(){
        Messenger<bool>.RemoveListener(GameEvent.OPERATE_ON_LIGHTS, OperateOnLights);
        Messenger.RemoveListener(GameEvent.ALL_MANAGERS_LOADED, ActivatePointLightManager); 
    }
    

}