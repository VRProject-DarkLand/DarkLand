using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof (CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour{    
    private const float WALK_SPEED = 3.0f;
    private const float CROUCH_SPEED = 2.0f;
    private const float RUN_SPEED = 5.5f;
    public float speed = 4.0f;
    private float jumpSpeed = 10f;
    private float minFall = -1.5f;
    private float _ySpeed ;
    private float gravity = -9.81f;
    private float airResistance = -0.2f;
    private float terminalVelocity = -15f;
    private float deltaX = 0;
    private float deltaZ = 0;
    private float dirX; 
    private float dirZ; 
    private bool onGround = false;
    private bool crouch = false;
    private CharacterController _charController;
    private ControllerColliderHit _contact; 
    private Camera _camera;
    private List<Actions> actions;
    private Quaternion jumpRotation;
    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();   
        _camera = GetComponentInChildren<Camera>();
        actions = new List<Actions>(){Actions.Idle};
        gameObject.tag = Settings.PLAYER_TAG;
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
            _ySpeed = jumpSpeed;
            jumpRotation = transform.rotation;
        }
    }
    private void Idle(){
        crouch = false;
        //_camera.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        speed = FPSInput.WALK_SPEED;     
    }

    private void Run(){
        if(onGround){
            speed = FPSInput.RUN_SPEED;  
            //_camera.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            crouch = false;
        }
    }

    private void MovePlayer(Vector3 movement) {
        //transform the movement from local to global coordinates
        if(onGround)
            movement = transform.TransformDirection(movement);
        else{
            Quaternion rot = transform.rotation;
            transform.rotation = jumpRotation;
            movement = transform.TransformDirection(movement);
            transform.rotation = rot;
        }
        _charController.Move(movement);
        // if ((!onGround) && (flags & CollisionFlags.Below) != 0){
        //     actions.RemoveAll(action => action == Actions.Jump);
        //     onGround = true;
        // }
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

    } 

    private bool detectOnGround(){
        //  onGround = false;
        //  if (( _charController.collisionFlags & CollisionFlags.Below) != 0){
        //     //actions.RemoveAll(action => action == Actions.Jump);
        //     onGround = true;
        // }
        bool hitGround = false;
        RaycastHit hit;
        if(_ySpeed < 0 && Physics.Raycast(transform.position ,Vector3.down, out hit)){
            float check = (_charController.height+ _charController.radius)/1.9f;
            hitGround = hit.distance <= check;
        }
        return hitGround;
    }

    // Update is called once per frame
    void Update()
    {
        onGround = detectOnGround();
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
            if (Input.GetButtonDown("Jump")){
                Jump();
            }else{
               //Debug.Log("CIAOUS");
                _ySpeed = minFall;
                deltaX = speed;
                deltaZ = speed;
            }
            
        }
        else {
            //we omit the friction when deciding to move 
            deltaX = Mathf.Max(0f, deltaX+airResistance*Time.deltaTime);
            deltaZ = Mathf.Max(0f, deltaZ+airResistance*Time.deltaTime);
            _ySpeed += gravity * 4f *Time.deltaTime;
            if ( _ySpeed < terminalVelocity){
                _ySpeed = terminalVelocity;
            }
           
        }
        //applying direction to X and Z delta
        Vector3 velocity = new Vector3(deltaX*dirX, 0, deltaZ*dirZ);
        velocity = Vector3.ClampMagnitude(velocity, speed);
        if(!onGround){
             if(_charController.isGrounded){
                if(Vector3.Dot(velocity, _contact.normal) < 0){
                    velocity -= _contact.normal * speed;
                }
                else{
                    velocity += _contact.normal * speed;
                }
            }

        }

        velocity.y = _ySpeed;
        velocity *= Time.deltaTime;     
        MovePlayer(velocity);
    }

    void OnControllerColliderHit(ControllerColliderHit hit){
        _contact = hit;
    }

}
