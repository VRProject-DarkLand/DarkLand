using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _fearBar;
    [SerializeField] private Image _selectedUsable;
    // Start is called before the first frame update
    void Start()
    {
        Messenger<float, bool>.AddListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectedItem(){

    }

    public void OnHealthChanged(float health, bool damaged){
        _healthBar.value = _healthBar.maxValue * health/Managers.Player.maxHealth;
                
    }
    public void OnFearChanged(){

    }

    void OnDestroy(){
         Messenger<float, bool>.RemoveListener(GameEvent.CHANGED_HEALTH, OnHealthChanged);
    }
}
