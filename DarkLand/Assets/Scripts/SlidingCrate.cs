using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingCrate : IInteractableObject, IDataPersistenceSave{
    [System.Serializable]
    public class WeaponBoxData {
        public string name;
        public bool used;
    }

    private bool used = false;
    private bool isMoving = false;
    private float speed;
    private float timeCount;
    [SerializeField] private GameObject slidingCrate;
    [SerializeField] private bool requireKey = false;
    [SerializeField] private string key = "Key";
    private Vector3 open;
    private Vector3 close;
    public float openingRange = 0.6f;
    private Transform center;
    // Start is called before the first frame update
    void Start()
    {

        interactableTrigger = GetComponent<InteractableTrigger>();
        if (!requireKey)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_AMMO_BOX);
        else
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.UNLOCK_AMMO_BOX);
        
        center = transform.Find("center");
    }

    public override bool CanInteract()
    {
        if (slidingCrate != null)
        {
            if (requireKey)
            {
                if (Managers.Inventory.GetItemCount(key) == 0)
                    return false;
            }
        }
        return true;

    }

    public override void Interact(){

        Debug.Log("Called interact");
        if(used)
            return;
        if (!isMoving)
        {
            if(interactionSound != null)
                GetComponent<AudioSource>().PlayOneShot(interactionSound);
            ChangeState();
            GameObject drop = Managers.DropsManager.DropRandomItem();
            drop.transform.position = center.position;
            
            interactableTrigger.RemoveFromInteractables();
            GetComponent<InteractableTrigger>().enabled = false;
        }
    }


    public void ChangeState()
    {
        if (slidingCrate != null )
        {
            if (requireKey)
            {
                requireKey = false;
                Managers.Inventory.ConsumeItem(key);
                interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_AMMO_BOX);
                return;
            }
            used = true;
            isMoving = true;
            StartCoroutine(AnimateDoor());
        }
    }

    private IEnumerator AnimateDoor(){
        if (slidingCrate != null){
            close = slidingCrate.transform.position;
            open = slidingCrate.transform.position + slidingCrate.transform.forward * openingRange;
        }
        speed = 1f;
        timeCount = 0;
        while (isMoving){
            slidingCrate.transform.position = Vector3.Lerp(close, open, timeCount * speed);
           
            timeCount += Time.deltaTime;
            if (slidingCrate.transform.position == open){
              isMoving = false;
              timeCount = 0;
            }
            yield return null;
        }       
    }

    public void SaveData(){
        WeaponBoxData data = new WeaponBoxData();
        data.name = gameObject.name;
        data.used = used;
        Settings.gameData.weaponBoxes.Add(data);
    }
}
