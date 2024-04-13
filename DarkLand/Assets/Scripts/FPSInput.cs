using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof (CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour{    
    private const float WALK_SPEED = 6.0f;
    private const float RUN_SPEED = 9.0f;
    public float speed = 6.0f;
    public float jumpHeight = 2f;
    private float gravity = -9.8f;
    private float airResistance = -0.2f;
    private float deltaY = 0;
    private float deltaX = 0;
    private float deltaZ = 0;
    private float dirX; 
    private float dirZ; 
    private bool onGround = false;
    private bool crouch = false;
    private CharacterController _charController;
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();   
        _camera = GetComponentInChildren<Camera>();
    }


    private void resetVelocity(){
        dirX =  Input.GetAxis("Horizontal");
        dirZ =  Input.GetAxis("Vertical");
        if(Input.GetKeyDown(KeyCode.C)){
            if(!crouch){
                crouch = true;
                _camera.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            }
        }else if(Input.GetKeyUp(KeyCode.C)){
            if(crouch){
                crouch = false;
                _camera.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            }
        }
        if (Input.GetKey(KeyCode.LeftShift)){
            speed = FPSInput.RUN_SPEED;
        }else{
           speed = FPSInput.WALK_SPEED;
        }
        deltaX = speed;
        deltaZ = speed; 
        if (Input.GetKeyDown(KeyCode.Space)){
            onGround = false;
            deltaY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    } 
                


    private void MovePlayer(Vector3 movement) {
        //transform the movement from local to global coordinates
        movement = transform.TransformDirection(movement);
        CollisionFlags flags = _charController.Move(movement);
        if ((flags & CollisionFlags.Below) != 0){
            onGround = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        if (onGround){
            resetVelocity();
        }
        else {
            //we omit the friction when deciding to move 
            deltaX = Mathf.Max(0f, deltaX+airResistance*Time.deltaTime);
            deltaZ = Mathf.Max(0f, deltaZ+airResistance*Time.deltaTime);
        }

        deltaY += gravity * Time.deltaTime;
        //applying direction to X and Z delta
        Vector3 velocity = new Vector3(deltaX*dirX, 0, deltaZ*dirZ);
        velocity = Vector3.ClampMagnitude(velocity, speed);
        //warning, clamp over y, when should we do it?
        velocity.y = deltaY;
        velocity *= Time.deltaTime;     
        MovePlayer(velocity);
    }


}
