using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    private float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = "Time: " + currentTime.ToString("f2");

        if (GameManager.Instance == null || GameManager.Instance.Phase != GamePhase.Playing)
            return;

        currentTime += Time.deltaTime;
    }
}
