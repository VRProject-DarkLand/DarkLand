using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyBear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// if it hitted an object whose parent has a ScaryGirlAI component, it call hitbyteddybear on it
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has the ScaryGirlAI script attached
        ScaryGirlAI scaryGirlAI = collision.gameObject.GetComponentInParent<ScaryGirlAI>();
        if (scaryGirlAI != null)
        {
            scaryGirlAI.HitByTeddyBear();
        }
    }

}
