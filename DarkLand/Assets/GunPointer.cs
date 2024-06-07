using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunPointer : MonoBehaviour{
   [SerializeField] Sprite _normalPointer;
   [SerializeField] Sprite _damagePointer;
    private Image _pointerImage;
    void Awake(){
        Messenger<bool>.AddListener(GameEvent.IS_USING_GUN, UpdatePointerVisibility);
        Messenger.AddListener(GameEvent.ENEMY_DAMAGED, SetDamageImage);
    }

    void Start(){
        _pointerImage = GetComponent<Image>();
        _pointerImage.enabled = false;
    }

    void UpdatePointerVisibility(bool active){
        if(active){
            _pointerImage.enabled = true;
            _pointerImage.sprite =  _normalPointer;
        }else{
            _pointerImage.enabled = false;
        }
    }
    void SetDamageImage(){
        StartCoroutine(PlaceDamageImage());
    }
    private IEnumerator PlaceDamageImage(){
        _pointerImage.sprite =  _damagePointer;
        yield return new WaitForSeconds(0.2f);
        _pointerImage.sprite =  _normalPointer;
    }

    void OnDestroy(){
        Messenger<bool>.RemoveListener(GameEvent.IS_USING_GUN, UpdatePointerVisibility);
        Messenger.RemoveListener(GameEvent.ENEMY_DAMAGED, SetDamageImage);
    
    }
}
