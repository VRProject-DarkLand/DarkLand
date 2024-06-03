using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingWolves : MonoBehaviour
{
    private Animator _animator;
    public float speed = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        _animator= GetComponent<Animator>();
        //_animator.SetBool("isRunning", true);
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool("isMoving", true);
        //transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
