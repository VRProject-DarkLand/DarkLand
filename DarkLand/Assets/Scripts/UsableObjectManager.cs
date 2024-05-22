using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UsableObjectManager : MonoBehaviour, IGameManager{

    private IUsableObject _currentObject;
    private int _currentIndex;
    private List<IUsableObject> _selectable;
    private IUsableObject _usableDummy;
    [SerializeField] private int _maxSize = 6;

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
       status = ManagerStatus.Started;
       Messenger<bool>.AddListener(GameEvent.IS_HIDING, SetActivationState);
    }

    //add element to selectable in the first free position
    public bool AddSelectable(GameObject obj){
        obj.SetActive(false);
        obj.transform.SetParent(null, true);
        GameObject usableObject = Instantiate(obj, _usableParent.transform, true); 
        usableObject.name = obj.name;
        if(obj == null)
            return false;
        for(int i = 0; i < _maxSize; ++i){
            if(_selectable[i].IsDummy()){
                _selectable[i] = usableObject.GetComponent<IUsableObject>();
                if(_currentIndex == i){
                    _currentObject = _selectable[_currentIndex];
                    _currentObject.Select();
                    Debug.Log("Pistol position" + _currentObject.transform.localPosition +" Rotation " + _currentObject.transform.localEulerAngles);
                }
                
                return true;
            }
        }
        return false;
    }
    //remove element from selectable if present (replace with dummy)
    public bool RemoveSelectable(GameObject obj){
        for(int i = 0; i < _selectable.Count; ++i){
            if(_selectable[i].gameObject == obj){
                if(_currentIndex == i){
                    _currentObject.Deselect();
                }
                _selectable[i] = _usableDummy;
                _currentObject = _usableDummy;
                return true;
            }
        }
        return false;
    }

    public void SelectionForward(){
        Debug.Log("Forw");
        if(_currentIndex < _selectable.Count -1){
            _currentObject.Deselect();
            ++_currentIndex;
            _currentObject = _selectable[_currentIndex];
        }else{
            _currentObject.Deselect();
            _currentObject = _selectable[0];
            _currentIndex = 0;
        }
        Debug.Log("Item n: "+_currentIndex);
        _currentObject.Select();
    }

    public void SelectionBackward(){
        Debug.Log("Back");
        if(_currentIndex > 0){
            _currentObject.Deselect();
            --_currentIndex;
            _currentObject = _selectable[_currentIndex];
        }else{
            _currentObject.Deselect();
            _currentObject = _selectable[_selectable.Count - 1];
            _currentIndex = _selectable.Count - 1;
        }
        Debug.Log("Item n: "+_currentIndex);
        _currentObject.Select();
    }

    public void Use(){
        _currentObject.Use();
        if( !_currentObject.IsDummy() && Managers.Inventory.GetItemCount(_currentObject.gameObject.name)==0){
            GameObject go = _currentObject.gameObject;
            RemoveSelectable(go);
            Destroy(go);
            
        }else {
           // _currentObject.gameObject.SetActive(false);
        }
         string tot = "Usable objects: ";
         for(int i = 0; i < _selectable.Count; ++i){
            tot+=" "+_selectable[i].gameObject.name;
         }
        Debug.Log(tot);
    }

    public void SecondaryUse(){
        _currentObject.SecondaryUse();
    }
        public void UndoSecondaryUse(){
        _currentObject.UndoSecondaryUse();
    }

    private void SetActivationState(bool isHiding){
            _currentObject.gameObject.SetActive(isHiding);
    }
    void Start(){
        
    }

    void Update()
    {
        
    }
    void OnDestroy(){
       Messenger<bool>.RemoveListener(GameEvent.IS_HIDING, SetActivationState);

    }
}
