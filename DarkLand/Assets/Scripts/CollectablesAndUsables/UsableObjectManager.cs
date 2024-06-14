using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UsableObjectManager : MonoBehaviour, IGameManager, IDataPersistenceSave{

    private IUsableObject _currentObject;
    private int _currentIndex;
    private List<IUsableObject> _selectable;
    private IUsableObject _usableDummy;
    private bool _active = false;
    public int _maxSize {get; private set;} = 7;

    [SerializeField] private GameObject _usableParent;
    public ManagerStatus status {get; private set;}
    
    //create list of dummy usable of fixed size
    public void Startup(){
        _selectable =  new List<IUsableObject>();
        _usableDummy = new GameObject().AddComponent<UsableDummy>();
        for(int i = 0; i < _maxSize; ++i){
            _selectable.Add(_usableDummy);
        }
        _currentObject = _selectable[0];
        _currentIndex = 0;
        //to be checked at load
        _active = false;
       status = ManagerStatus.Started;
       Messenger<bool>.AddListener(GameEvent.IS_HIDING, SetActivationState);
       Messenger<bool>.AddListener(GameEvent.SHOW_INVENTORY, SetActivationState);
    }

    //add element to selectable in the first free position
    public bool AddSelectable(GameObject obj){
        //obj.transform.SetParent(null, true);
       
        GameObject usableObject = Instantiate(obj, _usableParent.transform, false);
        Collectable collectable = usableObject.GetComponent<Collectable>();
        //obj.SetActive(false);
        usableObject.name = obj.name;
        if(obj == null){
            collectable.Collected = false;
            return false;
        }
        for(int i = 0; i < _maxSize; ++i){
            if(_selectable[i].IsDummy()){
                _selectable[i] = usableObject.GetComponent<IUsableObject>();
                foreach(Renderer r in usableObject.GetComponentsInChildren<Renderer>()){
                    r.gameObject.layer = LayerMask.NameToLayer("Usable");
                }
                if(_currentIndex == i){
                    _currentObject = _selectable[_currentIndex];
                    _currentObject.Position();
                    SetCurrentSelection(true);
                    Messenger<int>.Broadcast(GameEvent.CHANGED_SELECTABLE, i, MessengerMode.DONT_REQUIRE_LISTENER);
                    //Debug.Log("Pistol position" + _currentObject.transform.GetChild(0).localPosition +" Rotation " + _currentObject.transform.GetChild(0).localEulerAngles);
                }else{
                    _selectable[i].Position();
                    _selectable[i].Deselect();
                }
                Messenger<string, int>.Broadcast(GameEvent.USABLE_ADDED, usableObject.name, i, MessengerMode.DONT_REQUIRE_LISTENER);
                //set layer for usable items s.t. they are displayed from the usable objects camera.
                //In other words, they are displayed on top of everything

                collectable.Collected = true;
                return true;
            }
        }
        collectable.Collected = false;
        return false;
    }
    //remove element from selectable if present (replace with dummy)
    public bool RemoveSelectable(GameObject obj){
        for(int i = 0; i < _selectable.Count; ++i){
            if(_selectable[i].gameObject == obj){
                if(_currentIndex == i){
                    SetCurrentSelection(false);
                }
                _selectable[i] = _usableDummy;
                _currentObject = _usableDummy;
                foreach(Renderer r in obj.GetComponentsInChildren<Renderer>()){
                    r.gameObject.layer = LayerMask.NameToLayer("Default");
                }
                return true;
            }
        }
        return false;
    }

    public void SelectionBackward(){
        if(GameEvent.isHiding || GameEvent.isInventoryOpen)
            return;
        if(_currentIndex < _selectable.Count -1){
            SetCurrentSelection(false);
            ++_currentIndex;
            _currentObject = _selectable[_currentIndex];
        }else{
            SetCurrentSelection(false);
            _currentObject = _selectable[0];
            _currentIndex = 0;
        }
        //Debug.Log("Item n: "+_currentIndex);
        Messenger<int>.Broadcast(GameEvent.CHANGED_SELECTABLE, _currentIndex, MessengerMode.DONT_REQUIRE_LISTENER);
        SetCurrentSelection(true);
    }

    public void SelectionForward(){
        if(GameEvent.isHiding ||  GameEvent.isInventoryOpen)
            return;
        if(_currentIndex > 0){
            SetCurrentSelection(false);
            --_currentIndex;
            _currentObject = _selectable[_currentIndex];
        }else{
            SetCurrentSelection(false);
            _currentObject = _selectable[_selectable.Count - 1];
            _currentIndex = _selectable.Count - 1;
        }
        //Debug.Log("Item n: "+_currentIndex);
        Messenger<int>.Broadcast(GameEvent.CHANGED_SELECTABLE, _currentIndex, MessengerMode.DONT_REQUIRE_LISTENER);
        SetCurrentSelection(true);
    }

    public void Use(){
        if(!_active)
            return;
        _currentObject.Use();
        if (_currentObject.IsDummy())
            return;
        Messenger<string, int>.Broadcast(GameEvent.USED_USABLE, _currentObject.gameObject.name,  _currentIndex, MessengerMode.DONT_REQUIRE_LISTENER);
        if( Managers.Inventory.GetItemCount(_currentObject.gameObject.name)==0){
            GameObject go = _currentObject.gameObject;
            RemoveSelectable(go);
            Destroy(go);
            
        }else {
           // _currentObject.gameObject.SetActive(false);
        }
        //  string tot = "Usable objects: ";
        //  for(int i = 0; i < _selectable.Count; ++i){
        //     tot+=" "+_selectable[i].gameObject.name;
        //  }
        // Debug.Log(tot);
    }

    public void SecondaryUse(){
        _currentObject.SecondaryUse();
    }
        public void UndoSecondaryUse(){
        _currentObject.UndoSecondaryUse();
    }

    private void SetActivationState(bool isHiding){
        if(!isHiding){
            SetCurrentSelection(false);
        }
        else{
            SetCurrentSelection(true);
        }
    }
    private void SetCurrentSelection(bool toActivate){
        if(toActivate){
            _active = true;
            _currentObject.Select();

        }else{
            _active = false;
            _currentObject.Deselect();
        }
    }
    void OnDestroy(){
        Messenger<bool>.RemoveListener(GameEvent.IS_HIDING, SetActivationState);
        Messenger<bool>.RemoveListener(GameEvent.SHOW_INVENTORY, SetActivationState);
    }

    public void SaveData(){
        //save name of object and position in such a way that on load
        //the same object will end up in the same position
        for(int i = 0; i < _maxSize; ++i){
            if(_selectable[i].IsDummy()){
               Settings.gameData.usableItemsNames.Add("empty"); 
            }else{
                Settings.gameData.usableItemsNames.Add(_selectable[i].name);
            }
        }
    }
}
