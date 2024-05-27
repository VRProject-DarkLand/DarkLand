using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes{
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseX;
    public float sensitivityHor = 9.0f;
    public float sensitivityVer = 9.0f;

    public float minimumVert = -60f;
    public float maximumVert = 60f;

    private float _rotationX = 0;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null){
            body.freezeRotation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameEvent.isInDialog || Managers.Pause.paused )
            return;
        
        if(axes == RotationAxes.MouseX){
            if( GameEvent.isHiding)
            {return;}
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
        }
        else if(axes == RotationAxes.MouseY){

            _rotationX -=  Input.GetAxis("Mouse Y") * sensitivityVer;
            
            if(!GameEvent.isHiding)
                _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
            else 
                _rotationX = Mathf.Clamp(_rotationX, -10, 10);
            
            float rotationY = transform.localEulerAngles.y;
            
            transform.localEulerAngles = new Vector3(_rotationX,rotationY,0);
        }
        else {
            _rotationX -=  Input.GetAxis("Mouse Y") * sensitivityVer;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            float delta = Input.GetAxis("Mouse X") * sensitivityHor;
            float rotationY = transform.localEulerAngles.y + delta;
            
            transform.localEulerAngles = new Vector3(_rotationX,rotationY,0);
        }
    }
    void OnGUI(){
        if(GameEvent.isUsingGun){
            // Set the style for the GUI label (for the "+")
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 30; // Set the font size
            style.alignment = TextAnchor.MiddleCenter; // Set the alignment to center

            // Get the center of the screen
            float centerX = Screen.width / 2;
            float centerY = Screen.height / 2;

            // Draw the "+" symbol at the center of the screen
            GUI.Label(new Rect(centerX - 15, centerY - 15, 30, 30), "+", style);
        }
    }
}
