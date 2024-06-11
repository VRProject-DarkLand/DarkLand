using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Build.Content;
using UnityEditor.VersionControl;
using UnityEngine;
[RequireComponent(typeof (CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour, IDataPersistenceSave{    
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
    public bool crouch {get; private set;} = false;
    private bool hide;
    private CharacterController _charController;
    private Camera _camera;
    [SerializeField] private GameObject head;
    [SerializeField] private AudioClip footStepSound;
    private List<Actions> actions;
    private ControllerColliderHit _contact; 
    private Quaternion _jumpRotation;
    private Vector3 standingHead;
    private Vector3 crouchHead;
    private bool blocked = false;
    public int _health{get;private set;}
    private bool alive;
    private float _footStepSoundLength;
    private bool _step = true;
    private bool moveAction ;
    private AudioSource _soundSource;
    private float _maxJumpingAngle = 45f;
    private float _maxNonSlidingAngle = 40f;
    private RaycastHit _bottomHit;

    // Start is called before the first frame update
    void Start()
    {
        _footStepSoundLength = 0.3f;
        _soundSource = GetComponent<AudioSource>();
        _health = Managers.Player.maxHealth;
        _charController = GetComponent<CharacterController>();   
        _camera = GetComponentInChildren<Camera>();
        actions = new List<Actions>(){Actions.Walk};
        gameObject.tag = Settings.PLAYER_TAG;
        //Messenger<bool>.AddListener(GameEvent.IS_HIDING, Hide);
        hide = false;
        moveAction = false;
        alive = !Managers.Player.dead;
        standingHead = head.transform.localPosition;
        crouchHead = standingHead+new Vector3(0f, -0.3f, 0f);
    }


    public void SetSaveData(){
        _health  = Settings.gameData.playerHealth;
        if(!GameEvent.exitingCurrentScene){
            transform.position = Settings.gameData.playerPosition;
            transform.localEulerAngles = Settings.gameData.playerRotation;
        }
    }
    private enum Actions{
        Walk = 0,
        Crouch = 1,
        Run = 2, 
        Jump = 3
    } 

    private void setCamera(Vector3 position){
        
        //if(!GameEvent.isHiding){
            head.transform.localPosition = position;
        //}
    }

                
    private void Crouch(){
        if(onGround){
              crouch = true;
              setCamera(crouchHead);
              speed = FPSInput.CROUCH_SPEED;
              _footStepSoundLength = 0.7f;
              _soundSource.volume = 0.5f;
        } 
    }
    private float ComputeGroundSlope(){
        return Vector3.Angle(_bottomHit.normal, Vector3.up);
    }
    private void Jump(){
        if(!crouch && onGround){    
            float groundSlope = ComputeGroundSlope();
            if(groundSlope <= _maxJumpingAngle){
                onGround = false;
                deltaY = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _jumpRotation = transform.rotation;
            }
        }
    }
    private void Walk(){
        crouch = false;
        setCamera(standingHead);
        speed = FPSInput.WALK_SPEED;
        _soundSource.volume = 0.75f;
        _footStepSoundLength = 0.5f;
    }

    private void Run(){
        if(onGround){
            speed = FPSInput.RUN_SPEED;  
            setCamera(standingHead);
            _footStepSoundLength = 0.32f;
            _soundSource.volume = 1f;
            crouch = false;
        }
    }

    private IEnumerator WaitForFootSteps(){
        _step = false;
        yield return new WaitForSeconds(_footStepSoundLength);
        _step = true;
    }

    private void MovePlayer(Vector3 movement) {
        if (blocked)
            return;

         if (_charController.velocity.magnitude > 1f && _step && moveAction) {
            _soundSource.PlayOneShot(footStepSound);
            StartCoroutine(WaitForFootSteps());
        }
        _charController.Move(movement);
    }

    private void readActionFromInput(){
        if (onGround){
            dirX =  Input.GetAxis("Horizontal");
            dirZ =  Input.GetAxis("Vertical");
        }
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            actions.Add(Actions.Crouch);
        }else if(Input.GetKeyUp(KeyCode.LeftControl)){
            actions.RemoveAll(x => x == Actions.Crouch);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift)){    
            actions.Add(Actions.Run);
        }else if(Input.GetKeyUp(KeyCode.LeftShift)){
            actions.RemoveAll(x => x == Actions.Run);
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
                Managers.Pause.Pause();
        }

        if(Input.GetKeyDown(KeyCode.Tab)){
                Managers.Inventory.ChangeInventoryVisibility();
        }
        

        if(onGround && ! blocked){
            if(Input.GetKeyDown(KeyCode.E)){
                InteractableManager.InteractWithSelectedItem(false);
            }
            if(Input.GetKeyDown(KeyCode.C)){
                InteractableManager.InteractWithSelectedItem(true);
            } 
            
           
        }

        float scroll = Input.mouseScrollDelta.y;
        if ( scroll > 0){ // do shit here for scroll up
            Managers.UsableInventory.SelectionForward();
        } else if (scroll < 0 ){ // do shit here for scroll down 
            Managers.UsableInventory.SelectionBackward();
        }
    } 

    private bool detectOnGround(){
        bool hitGround = false;
        if(deltaY <= 0 && Physics.Raycast(transform.position ,Vector3.down, out _bottomHit)){
            float check = (_charController.height + _charController.radius)/1f;
            hitGround = _bottomHit.distance <= check;
        }
        return hitGround;
    }

    private void Die(){
        alive = false;
        Messenger.Broadcast(GameEvent.PLAYER_DEAD, MessengerMode.DONT_REQUIRE_LISTENER);
    }

    public void IncreaseHealth(int amount){        
        _health = Math.Min(Managers.Player.maxHealth, _health+amount);
        Messenger<float,bool>.Broadcast(GameEvent.CHANGED_HEALTH, _health, false);
            
    }
    public void Hurt(int damage){
        _health -= damage;
        Managers.Player.AddFear(damage/2);
        Messenger<float,bool>.Broadcast(GameEvent.CHANGED_HEALTH, _health, true);
        //sound?
        if(_health <= 0){
            Die();
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        if(Managers.Pause.paused && alive){
            if(Input.GetKeyDown(KeyCode.Escape)){
                Managers.Pause.OnEscResume();
            }
            return;
        }
        moveAction = false;
        onGround = detectOnGround();
        readActionFromInput();
        if(!(GameEvent.isInDialog || GameEvent.isHiding)){
            if(Input.GetMouseButtonDown(0)){
                Managers.UsableInventory.Use();
            }

            if(Input.GetMouseButton(1)){
                Managers.UsableInventory.SecondaryUse();
            }
            if(Input.GetMouseButtonUp(1)){
                Managers.UsableInventory.UndoSecondaryUse();
            }
            Actions action = actions.Last();
            if(action == Actions.Walk){
                 Walk();
            }else if(action == Actions.Crouch){
                Crouch();
            }else{
                Run();
            }
        }else{
            dirX = 0;
            dirZ = 0;
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
                moveAction = dirX != 0 ||  dirZ != 0;
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
        float groundSlope = ComputeGroundSlope();
        if(onGround &&  groundSlope > _maxNonSlidingAngle){
            Vector3 downwardDirection = Vector3.ProjectOnPlane(Vector3.down, _bottomHit.normal).normalized; 
            velocity += downwardDirection;
        }
        
        velocity *= Time.deltaTime;  
        if(!GameEvent.isHiding)
            MovePlayer(velocity);
    }

    // private void Hide(bool hide){
    //     this.hide = hide;
    // }

    void OnControllerColliderHit(ControllerColliderHit hit){
        _contact = hit;
    }

    void OnDestroy(){
        //Messenger<bool>.RemoveListener(GameEvent.IS_HIDING, Hide);
    }


    public bool IsHiding()
    {
        return GameEvent.isHiding;
    }

    public bool toggleDetectedByLittleGirl(bool state) { 
            bool prev = blocked;
            blocked = state;
            return prev;
    }

    public void SaveData(){
        Settings.gameData.playerHealth =  _health;
        Settings.gameData.playerPosition = transform.position;
        Settings.gameData.playerRotation = transform.localEulerAngles;
    }
}
