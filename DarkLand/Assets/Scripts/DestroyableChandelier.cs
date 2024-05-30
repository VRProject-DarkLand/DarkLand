using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableChandelier : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fall()
    {
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ThrowableObject throwableObject = collision.gameObject.GetComponent<ThrowableObject>();
        if (throwableObject != null)
        {
            fall();
        }
        ScaryGirlAI scaryGirlAI = collision.gameObject.GetComponentInParent<ScaryGirlAI>();
        if(scaryGirlAI != null) { 
            scaryGirlAI.Die();
            return;
        }
        FPSInput user = collision.gameObject.GetComponent<FPSInput>();
        if(user != null) {
            user.Hurt(110);
        }
    }
}
