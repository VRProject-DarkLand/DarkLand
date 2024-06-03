using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _fearBar;
    [SerializeField] private Image damageImage;
    [SerializeField] private GameObject usableSlots;
    [SerializeField] private GameObject usableSlotContainer; 
    [SerializeField] private GameObject ConfirmationPopup;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private InventoryViewer inventory;

    [SerializeField] private GameObject SettingsOptions;
    private GameObject leftMenu;
    private List<UsableSpot> spots = new List<UsableSpot>();
    private int currentIndex = 0;
    private CursorLockMode currentCursorLock;
    private int animatingDamageTimeElapsed = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Messenger<string, int>.AddListener(GameEvent.USABLE_ADDED, AddUsableElement);
        Messenger<string, int>.AddListener(GameEvent.USED_USABLE, UsedElement);
        Messenger<bool>.AddListener(GameEvent.PAUSED, Paused);
        Messenger<bool>.AddListener(GameEvent.SHOW_INVENTORY, OnInventoryChange);
        Messenger.AddListener(GameEvent.PLAYER_DEAD,OnPlayerDead);
        Messenger<float, bool>.AddListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
        Messenger<int>.AddListener(GameEvent.CHANGED_SELECTABLE, OnSelectableChanged);
        for(int i = 0;i<Managers.UsableInventory._maxSize;++i){
            GameObject slot = Instantiate(usableSlotContainer);
            slot.transform.SetParent(usableSlots.transform, false);
            spots.Add(slot.GetComponent<UsableSpot>());
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(UsableSlots.GetComponent<RectTransform>());
        damageImage.enabled = false;
        inventory.gameObject.SetActive(false);
        ConfirmationPopup.SetActive(false);
        currentCursorLock = CursorLockMode.Locked;
        leftMenu = pauseMenu.transform.Find("LeftPanel")?.gameObject;
        Paused(false);
        UpdateUIOnSaveLoad();
    }

    private void UpdateUIOnSaveLoad(){
        OnHealthChanged(Managers.Player.health, false);
        OnFearChanged();
    }

    private void AddUsableElement(string name, int pos){
        Sprite sprite = ResourceLoader.GetImage(name);
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

    public void OnSettingsClick(){
        Managers.Pause.settings = true;
        leftMenu?.SetActive(false);
        SettingsOptions.SetActive(true);
    }

    public void OnClickBack(){
        Managers.Pause.settings = false;
        leftMenu?.SetActive(true);
        SettingsOptions.SetActive(false);
    }

    private void OnSelectableChanged(int index){
        spots[currentIndex].Deselect();
        currentIndex  = index;
        spots[currentIndex].Select();
    }

    public void Paused(bool paused){
        if(Managers.Pause.settings){
            OnClickBack();
            return;
        }
        
        Cursor.lockState = paused? CursorLockMode.Confined : currentCursorLock;
        if(currentCursorLock != CursorLockMode.Locked)
            Cursor.visible = true;
        else 
            Cursor.visible = paused;
        pauseMenu.SetActive(paused);
        ConfirmationPopup.SetActive(false);
    }

    public void OnQuit(){
        ConfirmationPopup.SetActive(true);
    }
    public void OnInventoryChange(bool isInventoryOpen){
        if(isInventoryOpen){
            inventory.Clean();
            GameEvent.isInventoryOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            currentCursorLock = Cursor.lockState;
            Cursor.visible = false;
            inventory.gameObject.SetActive(false);
        }else{
            inventory.gameObject.SetActive(true); 
            inventory.Show();
            GameEvent.isInventoryOpen = true;
            Cursor.lockState = CursorLockMode.Confined;
            currentCursorLock = Cursor.lockState;
            Cursor.visible = true;
        }
       
    }
    public void OnQuitConfirm(bool choice){
        if(choice){
            Managers.Pause.OnClickResume();
            SceneManager.LoadScene(Settings.MAIN_MENU);
        }else{
            ConfirmationPopup.SetActive(false);
        }
    }


    public void OnPlayerDead(){
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        deathMenu.SetActive(true);
    }

    void OnDestroy(){
        Messenger<bool>.RemoveListener(GameEvent.SHOW_INVENTORY, OnInventoryChange);
        Messenger.RemoveListener(GameEvent.PLAYER_DEAD,OnPlayerDead);
        Messenger<float, bool>.RemoveListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
        Messenger<string, int>.RemoveListener(GameEvent.USABLE_ADDED, AddUsableElement);
        Messenger<string, int>.RemoveListener(GameEvent.USED_USABLE, UsedElement);
        Messenger<int>.RemoveListener(GameEvent.CHANGED_SELECTABLE, OnSelectableChanged);
        Messenger<bool>.RemoveListener(GameEvent.PAUSED, Paused);
    }

}
