using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeCollectableItems : MonoBehaviour
{
    // List of spawn points
    [SerializeField] private List<GameObject> spawnPoints;

    // List of collectables
    [SerializeField] private List<GameObject> collectables;

    void Start()
    {
        List<GameObject> shuffledSpawnPoints = new List<GameObject>(spawnPoints);
        Shuffle(shuffledSpawnPoints);

        for (int i = 0; i < collectables.Count; i++)
        {
            collectables[i].transform.position = shuffledSpawnPoints[i].transform.position;
            Debug.Log(collectables[i] + "To: " + shuffledSpawnPoints[i]);
        }
    }

    // Function to shuffle a list (Fisher-Yates shuffle algorithm)
    private void Shuffle(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
