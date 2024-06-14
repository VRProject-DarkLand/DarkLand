using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChandelierDrop : MonoBehaviour
{
    [SerializeField] private GameObject door;
    private OpenDoubleDoor script;
    [SerializeField] private float endY=36;
    private float speed;
    private float timeCount;


    // Start is called before the first frame update
    void Start()
    {
        script = door.GetComponent<OpenDoubleDoor>();
        speed = 0.3f;
        timeCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (script != null)
        {
            if (script.IsOpened())
            {
                Vector3 startPos = transform.position;
                Vector3 endPos = new Vector3(startPos.x, endY, startPos.z);

                transform.position = Vector3.Lerp(startPos, endPos, timeCount * speed);
                timeCount += Time.deltaTime;



            }
        }
        
    }
}
