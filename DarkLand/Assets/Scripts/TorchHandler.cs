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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        torch.GetComponent<Light>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
          torch.GetComponent<Light>().enabled = !torch.GetComponent<Light>().enabled;
        } 
    }
    void OnGUI() {
        int size = 50;
        float posX = _camera.pixelWidth/2  -size/2;
        float posY = _camera.pixelHeight/2 -size/2;
        GUI.Label(new Rect(posX, posY, size, size), "+");
    }
}
