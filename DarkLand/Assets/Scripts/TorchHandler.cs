using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchHandler : MonoBehaviour
{
    private Camera _camera;
    public GameObject torch;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        torch.GetComponent<Light>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ! GameEvent.isInDialog){
          torch.GetComponent<Light>().enabled = !torch.GetComponent<Light>().enabled;
        } 
        if(GameEvent.isHiding)
            torch.GetComponent<Light>().enabled = false;
    }
}
