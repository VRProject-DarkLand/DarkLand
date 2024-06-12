using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _fearBar;
    [SerializeField] private Image damageImage;
    [SerializeField] private Image cureImage;
    [SerializeField] private GameObject usableSlots;
    [SerializeField] private GameObject usableSlotContainer; 
    [SerializeField] private GameObject ConfirmationPopup;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private InventoryViewer inventory;
    [SerializeField] private GameObject SavedNotify;
    [SerializeField] private GameObject SettingsOptions;
    private GameObject leftMenu;
    private List<UsableSpot> spots = new List<UsableSpot>();
    private int currentIndex = 0;
    private int animatingDamageTimeElapsed = 0;
    private Image currentImage;    
    // Start is called before the first frame update
    void Start()
    {
        Messenger<string, int>.AddListener(GameEvent.USABLE_ADDED, AddUsableElement);
        Messenger<string, int>.AddListener(GameEvent.USED_USABLE, UsedElement);
        Messenger<bool>.AddListener(GameEvent.PAUSED, Paused);
        Messenger<bool>.AddListener(GameEvent.SHOW_INVENTORY, OnInventoryChange);
        Messenger.AddListener(GameEvent.PLAYER_DEAD,OnPlayerDead);
        Messenger<float, bool>.AddListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
        Messenger<int>.AddListener(GameEvent.FEAR_CHANGED, OnFearChanged);
        Messenger<int>.AddListener(GameEvent.CHANGED_SELECTABLE, OnSelectableChanged);
        Messenger.AddListener(GameEvent.SAVE_FINISHED, ActivateSaveNotify);
        Messenger.AddListener(GameEvent.ALL_MANAGERS_LOADED, LoadedManagers);
        for(int i = 0;i<Managers.UsableInventory._maxSize;++i){
            GameObject slot = Instantiate(usableSlotContainer);
            slot.transform.SetParent(usableSlots.transform, false);
            spots.Add(slot.GetComponent<UsableSpot>());
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(UsableSlots.GetComponent<RectTransform>());
        damageImage.enabled = false;
        inventory.gameObject.SetActive(false);
        
        ConfirmationPopup.SetActive(false);
        leftMenu = pauseMenu.transform.Find("LeftPanel")?.gameObject;
        Paused(false);
        Managers.PointerManager.ForceLock();
    }

    private IEnumerator DisableNotify(){
        yield return new WaitForSeconds(5f);
        if(SavedNotify!=null)
            SavedNotify.SetActive(false);
    }

    private void ActivateSaveNotify(){
        if(SavedNotify!=null)
            SavedNotify.SetActive(true);
        StartCoroutine(DisableNotify());
    }

    private void LoadedManagers(){

        UpdateUIOnSaveLoad();
    }

    private void UpdateUIOnSaveLoad(){
        _healthBar.value = _healthBar.maxValue * Managers.Player.health/Managers.Player.maxHealth;
        OnFearChanged(0);
    }

    private void AddUsableElement(string name, int pos){
        Sprite sprite = ResourceLoader.GetImage(name);
        spots[pos].SetItem(sprite);
    }

    public void OnHealthChanged(float health, bool damaged){
        _healthBar.value = _healthBar.maxValue * health/Managers.Player.maxHealth;
        bool startFade = false;
        if(currentImage == null)
            startFade = true;
        else
            currentImage.enabled = false;
        if(damaged){
                damageImage.color = new Color(1,1,1,0.8f);
                currentImage = damageImage;
                animatingDamageTimeElapsed = 0;
        }else{
            cureImage.color = new Color(1,1,1,0.8f);
            currentImage = cureImage;
            animatingDamageTimeElapsed = 0;
        }
        currentImage.enabled = true;
        if(startFade){
            StartCoroutine(FadeAwayImage());
        }
    }

    private IEnumerator FadeAwayImage(){
        float a = 1;
        float speed = 0.015f;
        animatingDamageTimeElapsed = 0;
        while(a != 0){
            a = Mathf.Lerp(0.8f,0,speed*animatingDamageTimeElapsed);
            animatingDamageTimeElapsed++;
            currentImage.color= new Color(1,1,1,a);
            yield return new WaitForSeconds(0.1f);
        }
        currentImage.enabled = false;
        currentImage = null;
    }
    
    private void UsedElement(string name, int pos){
        if(Managers.Inventory.GetItemCount(name) <= 0){
            spots[pos].SetItem(null);
        }
    }

    public void OnFearChanged(int fear){
        _fearBar.value = fear;
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
        
        if(paused) 
            Managers.PointerManager.UnlockCursor();
        else
            Managers.PointerManager.LockCursor();

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
            Managers.PointerManager.LockCursor();
            inventory.gameObject.SetActive(false);
        }else{
            inventory.gameObject.SetActive(true); 
            inventory.Show();
            GameEvent.isInventoryOpen = true;
            Managers.PointerManager.UnlockCursor();
        }
       
    }
    public void OnQuitConfirm(bool choice){
        if(choice){
            Managers.Pause.OnClickResume();
            SceneManager.LoadScene(Settings.MAIN_MENU);
            Settings.LoadedFromSave = false;
            GameEvent.OpenedSceneDoor = false;
        }else{
            ConfirmationPopup.SetActive(false);
        }
    }


    public void OnPlayerDead(){
        Managers.PointerManager.UnlockCursor();
        deathMenu.SetActive(true);
    }

    void OnDestroy(){
        Messenger<bool>.RemoveListener(GameEvent.SHOW_INVENTORY, OnInventoryChange);
        Messenger.RemoveListener(GameEvent.PLAYER_DEAD,OnPlayerDead);
        Messenger<float, bool>.RemoveListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
        Messenger<string, int>.RemoveListener(GameEvent.USABLE_ADDED, AddUsableElement);
        Messenger<string, int>.RemoveListener(GameEvent.USED_USABLE, UsedElement);
        Messenger<int>.RemoveListener(GameEvent.FEAR_CHANGED, OnFearChanged);
        Messenger<int>.RemoveListener(GameEvent.CHANGED_SELECTABLE, OnSelectableChanged);
        Messenger<bool>.RemoveListener(GameEvent.PAUSED, Paused);
        Messenger.RemoveListener(GameEvent.ALL_MANAGERS_LOADED, LoadedManagers);
    }

}
