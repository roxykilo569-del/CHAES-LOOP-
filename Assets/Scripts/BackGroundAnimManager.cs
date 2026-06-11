using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundAnimManager : MonoBehaviour
{
    public float timer = 0;
    public GameEvent flashEvent;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 15s
        if(timer >= 15.0f)
        {
            flashEvent.Raise();
            timer=0;
        }

        // when the game begin
        if(GameManager.Instance.Phase == GamePhase.Playing)
            timer += Time.deltaTime;
    }
}
