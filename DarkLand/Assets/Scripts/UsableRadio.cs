using UnityEngine;

public class UsableRadio : IUsableObject
{
    [SerializeField] private string changeScene;
    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.6f);
        Position();

    }

    public override void Position()
    {
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.66f);
        gameObject.transform.localEulerAngles = new Vector3(0,90,0);
    }

    public override void Use(){
       Settings.GameFinished = true;
       ScenesController.instance.ChangeScene(changeScene);
    }
}
