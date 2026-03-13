using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // dingyi

    public Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            playerAnimator.SetTrigger("JumpTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            playerAnimator.SetTrigger("SlideTrigger");
        }
    }
}
