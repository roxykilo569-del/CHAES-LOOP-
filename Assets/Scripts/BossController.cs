using UnityEngine;

public class BossController : MonoBehaviour
{
    public static BossController Instance;

    [Header("Boss Object")]
    public GameObject bossObject;

    [Header("State")]
    public bool bossActive = false;

    private void Awake()
    {
        Instance = this;

        // 游戏开始时先关闭 Boss
        if (bossObject != null)
        {
            bossObject.SetActive(false);
            bossActive = false;
        }
    }

    private void Update()
    {
        // 测试用：按 B 打开 Boss
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShowBoss();
        }

        // 测试用：按 N 关闭 Boss
        if (Input.GetKeyDown(KeyCode.N))
        {
            HideBoss();
        }
    }

    public void ShowBoss()
    {
        if (bossObject == null) return;
        if (bossActive) return;

        bossObject.SetActive(true);
        bossActive = true;

        if (CameraDirector2D.Instance != null)
        {
            CameraDirector2D.Instance.PlayBossIntro();
        }

        if (ScanlineEffectController.Instance != null)
        {
            ScanlineEffectController.Instance.SetBossMode();
        }

        if (BossFilterController.Instance != null)
        {
            BossFilterController.Instance.EnterBossFilter();
        }
        if (BrokenScreenController.Instance != null)
        {
            BrokenScreenController.Instance.SetBossBroken();
        }

        Debug.Log("Boss 出现");
    }

    public void HideBoss()
    {
        if (bossObject == null) return;
        if (!bossActive) return;

        bossObject.SetActive(false);
        bossActive = false;

        if (CameraDirector2D.Instance != null)
        {
            CameraDirector2D.Instance.SetNormalCamera();
        }

        if (ScanlineEffectController.Instance != null)
        {
            ScanlineEffectController.Instance.SetNormal();
        }
        if (BossFilterController.Instance != null)
        {
            BossFilterController.Instance.ExitBossFilter();
        }
        if (BrokenScreenController.Instance != null)
        {
            BrokenScreenController.Instance.SetNormalBroken();
        }

        Debug.Log("Boss 消失");
    }

    public void ToggleBoss()
    {
        if (bossActive)
        {
            HideBoss();
        }
        else
        {
            ShowBoss();
        }
    }
}