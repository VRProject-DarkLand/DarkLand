using System.Collections;
using System.Collections.Generic;
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
    }

    //add element to selectable in the first free position
    public bool AddSelectable(GameObject obj){
        GameObject usableObject = Instantiate(obj, _usableParent.transform); 
        if(obj == null)
            return false;
        for(int i = 0; i < _maxSize; ++i){
            if(_selectable[i].IsDummy()){
                _selectable[i] = usableObject.GetComponent<IUsableObject>();
                if(_currentIndex == i){
                    _currentObject = _selectable[_currentIndex];
                    _currentObject.Select();
                }
                
                return true;
            }
        }
        return false;
    }
    //remove element from selectable if present (replace with dummy)
    public bool RemoveSelectable(IUsableObject obj){
        for(int i = 0; i < _selectable.Count; ++i){
            if(_selectable[i].gameObject == obj){
                if(_currentIndex == i){
                    _currentObject.Deselect();
                }
                _selectable[i] = _usableDummy;
                return true;
            }
        }
        return false;
    }

    public void SelectionForward(){
        if(_currentIndex < _selectable.Count -1){
            _currentObject.Deselect();
            ++_currentIndex;
            _currentObject = _selectable[_currentIndex];
        }else{
            _currentObject = _selectable[0];
        }
        _currentObject.Select();
    }

    public void SelectionBackward(){
        if(_currentIndex > 0){
            _currentObject.Deselect();
            --_currentIndex;
            _currentObject = _selectable[_currentIndex];
        }else{
            _currentObject = _selectable[_selectable.Count - 1];
        }
        _currentObject.Select();
    }

    public void Use(){
        _currentObject.Use();
    }

    void Start(){
        
    }

    void Update()
    {
        
    }
}
