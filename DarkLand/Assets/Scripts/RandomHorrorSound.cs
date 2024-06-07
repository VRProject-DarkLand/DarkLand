using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHorrorSound : MonoBehaviour
{
    public AudioClip[] sounds;

    void Start()
    {
        StartCoroutine(PickAudio());
    }

    IEnumerator PickAudio(){
        while(true){
            int randomIndex = Random.Range(0, sounds.Length);
            AudioClip randomClip = sounds[randomIndex];
            StartCoroutine(ReproduceAudio(randomClip));
            yield return new WaitForSeconds(Random.Range(40f, 120f));
        }
    }

    IEnumerator ReproduceAudio(AudioClip clip){
        Managers.AudioManager.PlaySound(clip,0.1f);
        yield return null;
    }
}
