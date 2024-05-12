using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
[RequireComponent(typeof (CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour{    
    private const float WALK_SPEED = 3.0f;
    private const float CROUCH_SPEED = 2.0f;
    private const float RUN_SPEED = 5.5f;
    public float speed = 4.0f;
    private float _minFall = -3f;
    private float jumpHeight = 1f;
    private float gravity = -9.8f;
    private float airResistance = -0.2f;
    private float _terminalVelocity = -25f;
    private float deltaY = 0;
    private float deltaX = 0;
    private float deltaZ = 0;
    private float dirX; 
    private float dirZ; 
    public bool onGround = false;
    private bool crouch = false;
    private CharacterController _charController;
    private Camera _camera;
    private List<Actions> actions;
    private ControllerColliderHit _contact; 
    private Quaternion _jumpRotation;
    public float temp;
    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();   
        _camera = GetComponentInChildren<Camera>();
        actions = new List<Actions>(){Actions.Walk};
        gameObject.tag = Settings.PLAYER_TAG;
    }


    private enum Actions{
        Walk = 0,
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
            _jumpRotation = transform.rotation;
        }
    }
    private void Walk(){
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

        if(Input.GetKeyDown(KeyCode.E)){
            InteractableManager.InteractWithSelectedItem();
        }
    } 

    private bool detectOnGround(){
        bool hitGround = false;
        RaycastHit hit;
        if(deltaY <= 0 && Physics.Raycast(transform.position ,Vector3.down, out hit)){
            float check = (_charController.height + _charController.radius)/1.3f;
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
        if(GameEvent.isInDialog){
            dirX = 0;
            dirZ = 0;
        }else if(action == Actions.Walk){
            Walk();
        }else if(action == Actions.Crouch){
            Crouch();
        }else{
            Run();
        }

        if(onGround){
            if (Input.GetButtonDown("Jump")){
                if(!GameEvent.isInDialog){
                    Jump();
                }
            }else{
                deltaY = _minFall;
                deltaX = speed;
                deltaZ = speed;
            }
        }
        else{
            //we omit the friction when deciding to move 
            deltaX = Mathf.Max(0f, deltaX+airResistance*Time.deltaTime);
            deltaZ = Mathf.Max(0f, deltaZ+airResistance*Time.deltaTime);
            if ( deltaY < _terminalVelocity){
                deltaY = _terminalVelocity;
            }
        }


        deltaY += gravity * Time.deltaTime;
        //applying direction to X and Z delta
        Vector3 velocity = new Vector3(deltaX*dirX, 0, deltaZ*dirZ);
        velocity = Vector3.ClampMagnitude(velocity, speed);        
        //warning, clamp over y, when should we do it?
        velocity.y = deltaY;
        if(onGround)
            velocity = transform.TransformDirection(velocity);
        else{
            Quaternion rot = transform.rotation;
            transform.rotation = _jumpRotation;
            velocity = transform.TransformDirection(velocity);
            transform.rotation = rot;
        }
        if(!onGround){
            if(_charController.isGrounded){
                temp =  Vector3.Dot(velocity, _contact.normal);
                if(Vector3.Dot(velocity, _contact.normal) < 0){
                    velocity = _contact.normal * speed;
                }
                else{
                    velocity += _contact.normal * speed;
                }
            }

        }
        velocity *= Time.deltaTime;  
        MovePlayer(velocity);
    }

    void OnControllerColliderHit(ControllerColliderHit hit){
        _contact = hit;
    }
}
