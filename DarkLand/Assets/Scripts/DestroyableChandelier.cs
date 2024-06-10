using System;
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
    void Update(){}

    public void fall()
    {
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ThrowableObject throwableObject = collision.gameObject.GetComponent<ThrowableObject>();
        if (throwableObject != null && ScaryGirlUnderChandelier())
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

    private bool ScaryGirlUnderChandelier()
    {
        float maxDistance = 100f;
        Vector3 boxSize = new Vector3(1.8f,1.8f,1.8f);
        // Calculate the box cast origin (start from the current object's position)
        Vector3 origin = transform.position;

        // Box cast direction (downwards)
        Vector3 direction = Vector3.down;

        // Perform the BoxCast
        RaycastHit[] hits = Physics.BoxCastAll(origin, boxSize / 2, direction, Quaternion.identity, maxDistance);

        // Check if any of the hits have the ScaryGirlAI component
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<ScaryGirlAI>() != null)
            {
                hit.collider.gameObject.GetComponent<ScaryGirlAI>().lockInPlace();
                return true;
            }
        }

        return false;
    }
}
