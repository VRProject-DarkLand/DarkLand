using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLightManager : MonoBehaviour
{
    private  GameObject player;
    private  Light[] allLights;

    void Start()
    {
        // Find the player GameObject (assuming it has a tag "Player")
        player = GameObject.FindGameObjectWithTag("Player");
        // Find all GameObjects with Light component
        allLights = GameObject.FindObjectsOfType<Light>();
        StartCoroutine(BlinkManager());
    }
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
            l.enabled = true;
        }
    }
    IEnumerator BlinkManager(){
        while(true){
            // Create a list to store the point lights and their distances
            List<KeyValuePair<Light, float>> lightDistances = new List<KeyValuePair<Light, float>>();
            // Calculate distances and store them along with the lights
            foreach (Light light in allLights)
            {
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
            Debug.Log("Randomly selected light: " + selectedLight.gameObject.name);
            StartCoroutine(Blink(selectedLight.gameObject));
            yield return new WaitForSeconds(Random.Range(3f, 12f));
        }
    }
}
