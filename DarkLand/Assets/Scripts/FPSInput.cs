using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof (CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour{    
    private const float WALK_SPEED = 6.0f;
    private const float CROUCH_SPEED = 3.0f;
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
    private List<Actions> actions;
    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();   
        _camera = GetComponentInChildren<Camera>();
        actions = new List<Actions>(){Actions.Idle};
    }


    private enum Actions{
        Idle = 0,
        Crouch = 1,
        Run = 2, 
        Jump = 3
    } 

                
    private void Crouch(){
        if(onGround){
              crouch = true;
              _camera.transform.localPosition = new Vector3(0f, 0.2f, 0f);
              speed = FPSInput.CROUCH_SPEED;
        } 
    }

    private void Jump(){
        if(!crouch && onGround){
            onGround = false;
            deltaY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    private void Idle(){
        crouch = false;
        _camera.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        speed = FPSInput.WALK_SPEED;     
    }

    private void Run(){
        if(onGround){
            speed = FPSInput.RUN_SPEED;  
            _camera.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            crouch = false;
        }
    }

    private void MovePlayer(Vector3 movement) {
        //transform the movement from local to global coordinates
        movement = transform.TransformDirection(movement);
        CollisionFlags flags = _charController.Move(movement);
        if ((!onGround) && (flags & CollisionFlags.Below) != 0){
            actions.RemoveAll(action => action == Actions.Jump);
            onGround = true;
        }
    }

        private void readActionFromInput(){
        if (onGround){
            dirX =  Input.GetAxis("Horizontal");
            dirZ =  Input.GetAxis("Vertical");
        }
        if(Input.GetKeyDown(KeyCode.C)){
            actions.Add(Actions.Crouch);
        }else if(Input.GetKeyUp(KeyCode.C)){
            actions.RemoveAll(x => x == Actions.Crouch);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift)){    
            actions.Add(Actions.Run);
        }else if(Input.GetKeyUp(KeyCode.LeftShift)){
            actions.RemoveAll(x => x == Actions.Run);
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }
    } 

    // Update is called once per frame
    void Update()
    {
        readActionFromInput();

        Actions action = actions.Last();

        if(action == Actions.Idle){
            Idle();
        }else if(action == Actions.Crouch){
            Crouch();
        }else{
            Run();
        }

        if(onGround){
            deltaX = speed;
            deltaZ = speed;
        }
        if(!onGround) {
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
