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
    private int animatingDamageTimeElapsed = 0;
    // Start is called before the first frame update
    void Start()
    {
        Messenger<float, bool>.AddListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
        damageImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectedItem(){

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

    public void OnFearChanged(){

    }

    void OnDestroy(){
         Messenger<float, bool>.RemoveListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
    }
}
