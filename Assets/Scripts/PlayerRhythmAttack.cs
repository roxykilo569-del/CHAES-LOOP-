using UnityEngine;

public class PlayerRhythmAttack : MonoBehaviour
{
    private const int KEY_Z = 0;
    private const int KEY_X = 1;
    private const int KEY_C = 2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TryJudge(KEY_Z);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            TryJudge(KEY_X);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            // C 是攻击，所以先打开攻击范围
            if (AttackRange2D.Instance != null)
            {
                AttackRange2D.Instance.BeginAttack();
            }

            TryJudge(KEY_C);
        }
    }

    private void TryJudge(int keyType)
    {
        if (RhythmJudgeManager.Instance != null)
        {
            RhythmJudgeManager.Instance.TryJudge(keyType);
        }
    }
}