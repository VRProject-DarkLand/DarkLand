using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : IUsableObject
{
    private static FPSInput player;
    void Start(){
        if(player == null)
            player = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG).GetComponent<FPSInput>();
    }
    public override void Use()
    {
        if(player != null){
            if(Managers.Inventory.GetItemCount(Settings.HEALTH)>0 && (Managers.Player.maxHealth > player._health || Managers.Player.fearLevel > 0))
            {
                Managers.Inventory.ConsumeItem(Settings.HEALTH); 
                Managers.Player.AddFear(-50);
                player.IncreaseHealth(50);
            }
        }
    }

    public override void Select()
    {
          gameObject.SetActive(true);
          Position();
    }
    public override void Position()
    {
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.66f);
        gameObject.transform.localEulerAngles = new Vector3(0,90,0);
    }

    public override bool IsDummy()
    {
        return false;
    }
}
