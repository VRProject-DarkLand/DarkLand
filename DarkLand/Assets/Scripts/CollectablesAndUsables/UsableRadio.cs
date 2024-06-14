using UnityEngine;

public class UsableRadio : IUsableObject
{
    [SerializeField] private string changeScene;
    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        gameObject.SetActive(true);
        Position();

    }

    public override void Position()
    {
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.66f);
        gameObject.transform.localEulerAngles = new Vector3(0,-90,0);
    }

    
    /// <summary>
    ///1Change scene to the scene passed as serializefield
    /// </summary>
    public override void Use(){
       Settings.GameFinished = true;
       ScenesController.instance.ChangeScene(changeScene);
    }
}
