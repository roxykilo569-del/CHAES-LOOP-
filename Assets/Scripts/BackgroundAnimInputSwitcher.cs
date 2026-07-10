using System.Collections;
using UnityEngine;

public class BackgroundAnimInputSwitcher : MonoBehaviour
{
    [Header("Target")]
    public Animator animator;

    [Header("Animator State Names")]
    public string idleState = "BOSS1";
    public string zState = "CR-Z";
    public string xState = "CR-X";
    public string cState = "CR-C";

    [Header("Keys")]
    public KeyCode zKey = KeyCode.Z;
    public KeyCode xKey = KeyCode.X;
    public KeyCode cKey = KeyCode.C;

    [Header("Return Time")]
    public float zReturnTime = 0.35f;
    public float cReturnTime = 0.25f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (animator == null) return;

        if (Input.GetKeyDown(zKey))
        {
            PlayOnceThenIdle(zState, zReturnTime);
        }

        if (Input.GetKeyDown(cKey))
        {
            PlayOnceThenIdle(cState, cReturnTime);
        }

        // X 一般是滑铲，所以这里做成按住时播放，松开恢复
        if (Input.GetKeyDown(xKey))
        {
            PlayState(xState);
        }

        if (Input.GetKeyUp(xKey))
        {
            PlayState(idleState);
        }
    }

    private void PlayOnceThenIdle(string stateName, float returnTime)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(PlayOnceRoutine(stateName, returnTime));
    }

    private IEnumerator PlayOnceRoutine(string stateName, float returnTime)
    {
        PlayState(stateName);

        yield return new WaitForSeconds(returnTime);

        PlayState(idleState);

        currentRoutine = null;
    }

    private void PlayState(string stateName)
    {
        animator.Play(stateName, 0, 0f);
    }
}