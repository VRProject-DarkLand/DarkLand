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
    public float sensitivityHor;
    public float sensitivityVer;

    public float minimumVert = -60f;
    public float maximumVert = 60f;
    private bool aim = false;
    private float _rotationX = 0;
    // Start is called before the first frame update
    void Start()
    {
        aim = false;
        sensitivityHor = Settings.Sensibility;
        sensitivityVer = Settings.Sensibility;
        Messenger<bool>.AddListener(GameEvent.AIMING, Aim);
        Messenger.AddListener(GameEvent.SENSIBILITY_CHANGE, ChangeSensibility);
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null){
            body.freezeRotation = true;
        }
    }

    private void ChangeSensibility(){
        sensitivityHor = aim? Settings.AimSensibility:Settings.Sensibility;
        sensitivityVer = sensitivityHor; 
    }



    private void Aim(bool aim){
        sensitivityHor = aim? Settings.AimSensibility:Settings.Sensibility;
        sensitivityVer = sensitivityHor; 
        this.aim = aim;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameEvent.isInDialog || Managers.Pause.paused || GameEvent.isInventoryOpen)
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

    void OnDestroy(){
        Messenger<bool>.RemoveListener(GameEvent.AIMING, Aim);
        Messenger.RemoveListener(GameEvent.SENSIBILITY_CHANGE, ChangeSensibility);
    }
}
