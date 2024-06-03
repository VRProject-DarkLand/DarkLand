using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingCrate : IInteractableObject
{
    private bool opened;
    private bool isMoving;
    private float speed;
    private float timeCount;
    [SerializeField] private GameObject slidingCrate;
    [SerializeField] private bool requireKey = false;
    [SerializeField] private string key = "Key";
    private Vector3 open;
    private Vector3 close;
    public float openingRange = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        if (slidingCrate != null)
        {
            opened = false;
            isMoving = false;
            close = slidingCrate.transform.position;
            open = slidingCrate.transform.position + Vector3.left * openingRange;
        }
        speed = 1f;
        timeCount = 0;
        interactableTrigger = GetComponent<InteractableTrigger>();
        if (!requireKey)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
        else
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.UNLOCK);
        
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

    public override void Interact()
    {
        if (!isMoving)
        {
            ChangeState();
            StartCoroutine(AnimateDoor());
        }
    }


    public void ChangeState()
    {
        if (slidingCrate != null )
        {
            if (CanInteract())
            {
                if (requireKey)
                {
                    requireKey = false;
                    Managers.Inventory.ConsumeItem(key);
                    interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
                    return;
                }
            }
            // if(opened){
            //     left.transform.rotation = close.Item1 ;
            //     right.transform.rotation = close.Item2 ;
            // }else{
            //     left.transform.rotation = open.Item1 ;
            //     right.transform.rotation = open.Item2 ;
            // }
            if (opened)
                interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
            else
                interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.CLOSE_DOOR);
            opened = !opened;
            isMoving = true;
        }
    }

    private IEnumerator AnimateDoor()
    {
        Vector3 begin = open;
        Vector3 end = close;
        if (opened)
        {
            begin = close;
            end = open;
        }
        while (isMoving)
        {
            slidingCrate.transform.position = Vector3.Lerp(begin, end, timeCount * speed);
           
            timeCount += Time.deltaTime;
            if (slidingCrate.transform.position == end)
            {
              isMoving = false;
              timeCount = 0;
            }
            yield return null;
        }
    }
    // Update is called once per frame
}
