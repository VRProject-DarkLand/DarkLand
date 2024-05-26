using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _fearBar;
    [SerializeField] private Image _selectedUsable;
    [SerializeField] private Image damageImage;
    [SerializeField] private GameObject UsableSlots;
    [SerializeField] private GameObject UsableSlotContainer; 
    private List<UsableSpot> spots = new List<UsableSpot>();
    private int currentIndex = 0;

    private int animatingDamageTimeElapsed = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Messenger<string, int>.AddListener(GameEvent.USABLE_ADDED, AddUsableElement);
        Messenger<string, int>.AddListener(GameEvent.USED_USABLE, UsedElement);
        Messenger<float, bool>.AddListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
        Messenger<int>.AddListener(GameEvent.CHANGED_SELECTABLE, OnSelectableChanged);
        for(int i = 0;i<Managers.UsableInventory._maxSize;++i){
            GameObject slot = Instantiate(UsableSlotContainer);
            slot.transform.SetParent(UsableSlots.transform, false);
            spots.Add(slot.GetComponent<UsableSpot>());
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(UsableSlots.GetComponent<RectTransform>());
        damageImage.enabled = false;
    }

    private void AddUsableElement(string name, int pos){
        Sprite sprite = Resources.Load<Sprite>("InventoryIcons/"+name);
        spots[pos].SetItem(sprite);
    }


    public void OnHealthChanged(float health, bool damaged){
        _healthBar.value = _healthBar.maxValue * health/Managers.Player.maxHealth;
        if(!damageImage.enabled && damaged){
            damageImage.color = new Color(1,1,1,0.8f);
            damageImage.enabled = true;
            StartCoroutine(FadeAwayImage());
        }
        else if(!damaged){
             damageImage.enabled = false;
        }
        else{
            animatingDamageTimeElapsed = 0;
        }
    }

    private IEnumerator FadeAwayImage(){
        float a = 1;
        float speed = 0.015f;
        animatingDamageTimeElapsed = 0;
        while(a != 0){
            a = Mathf.Lerp(0.8f,0,speed*animatingDamageTimeElapsed);
            animatingDamageTimeElapsed++;
            damageImage.color= new Color(1,1,1,a);
            yield return new WaitForSeconds(0.1f);
        }
        damageImage.enabled = false;
    }
    
    private void UsedElement(string name, int pos){
        if(Managers.Inventory.GetItemCount(name) <= 0){
            spots[pos].SetItem(null);
        }
    }

    public void OnFearChanged(){

    }

    private void OnSelectableChanged(int index){
        spots[currentIndex].Deselect();
        currentIndex  = index;
        spots[currentIndex].Select();
    }

    void OnDestroy(){
        Messenger<float, bool>.RemoveListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
        Messenger<string, int>.RemoveListener(GameEvent.USABLE_ADDED, AddUsableElement);
        Messenger<string, int>.RemoveListener(GameEvent.USED_USABLE, UsedElement);
        Messenger<int>.RemoveListener(GameEvent.CHANGED_SELECTABLE, OnSelectableChanged);
    }

}
