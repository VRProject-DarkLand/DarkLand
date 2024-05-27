using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomHorrorSound : MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PickAudio());
    }

    IEnumerator PickAudio(){
        while(true){
            int randomIndex = Random.Range(0, sounds.Length);
            AudioClip randomClip = sounds[randomIndex];
            StartCoroutine(ReproduceAudio(randomClip));
            yield return new WaitForSeconds(Random.Range(40f, 60f));
        }
    }

    IEnumerator ReproduceAudio(AudioClip clip){
        audioSource.PlayOneShot(clip,0.1f);
        yield return null;
    }
}
