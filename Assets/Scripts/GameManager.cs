using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

/// <summary>
/// 障碍触碰：第一次进入危险状态，第二次触碰则死亡并在复活点重生。
/// 需在场景中放置复活点 Transform，并指定玩家引用（或挂有 PlayerController 的物体）。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("玩家与复活(备用)")]
    [Tooltip("复活位置（出生点）。不填则使用玩家初始位置。")]
    public Transform respawnPoint;

    [Tooltip("不填则自动在场景中查找 PlayerController。")]
    public PlayerController player;

    [Header("Global Volume")]
    [Tooltip("名为 \"Global Volume\" 的对象上挂的 Volume 组件。留空则自动查找。")]
    public Volume globalVolume;
    [Tooltip("首次命中障碍后启用的权重。")]
    public float volumeActiveWeight = 1f;
    [Tooltip("默认权重（保持为 0 以便只在首次命中时生效）。")]
    public float volumeDefaultWeight = 0f;

    [Header("弹窗(UI)")]
    [Tooltip("名为 \"PopUp\" 的根对象。留空则自动查找。")]
    public GameObject popUpRoot;
    [Tooltip("弹窗里的重开按钮（名为 \"Button\"）。留空则自动查找。")]
    public Button restartButton;
    [Tooltip("弹窗文字（Text (TMP)）。留空则自动查找。")]
    public TextMeshProUGUI popUpText;

    [Header("音乐")]
    [Tooltip("通常挂在 Main Camera 上的背景音乐 AudioSource。留空则自动从 Camera.main 上获取。")]
    public AudioSource bgm;

    [Header("游戏阶段")]
    [SerializeField] private GamePhase phase = GamePhase.None;

    /// <summary>当前阶段（只读）。切换请用 SetPhase / SetGamePhase。</summary>
    public GamePhase Phase => phase;

    [Header("游玩子阶段（Playing 时）")]
    [SerializeField] private GamePlayPhase gamePlayPhase = GamePlayPhase.Normal;

    /// <summary>当前游玩子阶段（只读）。切换请用 <see cref="SetGamePlayPhase"/>。</summary>
    public GamePlayPhase PlayPhase => gamePlayPhase;

    [Header("Boss 模式 — 障碍当攻击")]
    [Tooltip("玩家处于 Boss 子阶段时，撞到障碍在此位置生成（可选）。留空则仅销毁障碍。")]
    public GameObject bossHitVfxPrefab;

    [Header("状态（只读）")]
    [SerializeField] private bool inDanger;

    public bool InDanger => inDanger;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (Instance == this)
            DialogueRuntimeEvents.EventRaised += OnDialogueRuntimeEvent;
    }

    void OnDisable()
    {
        if (Instance == this)
            DialogueRuntimeEvents.EventRaised -= OnDialogueRuntimeEvent;
    }

    /// <summary>
    /// 在 DialogueData / 各句里填写 eventId，在此根据 id 写逻辑（不要用资源上绑 UnityEvent）。
    /// </summary>
    void OnDialogueRuntimeEvent(string eventId, DialogueData dialogue, int lineIndex)
    {
        switch (eventId)
        {
            // 示例：对话资源里 dialogueStartEventId 填 "Game_Start"
            case "Game_Start":
                SetPhase(GamePhase.Playing);
                break;
            case "Boss_Start":
                SetGamePlayPhase(GamePlayPhase.Boss);
                break;
            case "Boss_End":
                SetGamePlayPhase(GamePlayPhase.Normal);
                break;
            default:
                break;
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        AcquireReferences();
        //ResetStateForScene();
        BindRestartButton();
    }

    /// <summary>
    /// 切换游戏阶段（对外接口）。例：准备结束开始游玩 <c>SetPhase(GamePhase.Playing)</c>，打开暂停菜单 <c>SetPhase(GamePhase.Paused)</c>。
    /// </summary>
    public void SetPhase(GamePhase newPhase)
    {
        if (phase == newPhase) return;
        phase = newPhase;
        ApplyPhase(newPhase);
    }

    public void SetPrepareState()
    {
        if(phase == GamePhase.Preparation) return;
        phase = GamePhase.Preparation;
        ApplyPhase(phase);
    }

    /// <summary>与 SetPhase 相同，便于在 Button 上绑定。</summary>
    public void SetGamePhase(GamePhase newPhase) => SetPhase(newPhase);

    /// <summary>切换游玩子阶段（Normal / Boss）。</summary>
    public void SetGamePlayPhase(GamePlayPhase newPlayPhase)
    {
        if (gamePlayPhase == newPlayPhase) return;
        gamePlayPhase = newPlayPhase;

        if (newPlayPhase == GamePlayPhase.Boss)
        {
            ClearDanger();
            if (globalVolume != null)
                globalVolume.weight = volumeDefaultWeight;
        }
    }

    public void OnPlayerHitObstacle()
    {
        if (phase != GamePhase.Playing) return;
        if (gamePlayPhase != GamePlayPhase.Normal) return;

        if (!inDanger)
        {
            inDanger = true;
            ActivateDangerVolume();
            return;
        }

        inDanger = false;
        SetPhase(GamePhase.Dead);
    }

    /// <summary>Boss 子阶段下玩家与障碍碰撞：不受伤，播放攻击表现并移除该障碍。</summary>
    public void OnPlayerBossHitObstacle(Obstacle obstacle)
    {
        if (phase != GamePhase.Playing) return;
        if (gamePlayPhase != GamePlayPhase.Boss) return;
        if (obstacle == null) return;

        Vector3 pos = obstacle.transform.position;
        if (bossHitVfxPrefab != null)
            Object.Instantiate(bossHitVfxPrefab, pos, Quaternion.identity);

        Destroy(obstacle.gameObject);
    }

    /// <summary>
    /// 需要时可在别处重置危险状态（例如过关、读档）。
    /// </summary>
    public void ClearDanger()
    {
        inDanger = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AcquireReferences();
        ResetStateForScene();
        BindRestartButton();
    }

    void AcquireReferences()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        if (globalVolume == null)
        {
            var go = GameObject.Find("Global Volume");
            if (go != null) globalVolume = go.GetComponent<Volume>();
        }

        if (popUpRoot == null)
        {
            popUpRoot = GameObject.Find("PopUp");
        }

        // GameManager 可能 DontDestroyOnLoad：重载场景后旧引用会失效，每次重新抓主摄像机上的音源
        if (!bgm)
        {
            var cam = Camera.main;
            if (cam != null)
                bgm = cam.GetComponent<AudioSource>();
        }

        if (popUpRoot != null)
        {
            if (restartButton == null)
            {
                // 弹窗里已存在一个 Button，这里只抓名为 “Button” 的那一个
                var btns = popUpRoot.GetComponentsInChildren<Button>(true);
                foreach (var b in btns)
                {
                    if (b != null && b.gameObject.name == "Button")
                    {
                        restartButton = b;
                        break;
                    }
                }
            }

            if (popUpText == null)
            {
                // TextMeshProUGUI 名称在场景里通常叫 “Text (TMP)”
                var tmp = popUpRoot.transform.Find("Button/Text (TMP)");
                if (tmp != null) popUpText = tmp.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    void ResetStateForScene()
    {
        // 复活点/出生点：保留你的旧逻辑作为备用
        if (player != null && respawnPoint != null)
            player.SetSpawnPoint(respawnPoint.position);

        inDanger = false;
        gamePlayPhase = GamePlayPhase.Normal;
        // 场景每次加载都要刷新 UI/时间缩放；若仅用 SetPhase(Preparation)，首次与上次同为 Preparation 时会早退
        phase = GamePhase.Preparation;
        ApplyPhase(GamePhase.Preparation);
    }

    void ApplyPhase(GamePhase p)
    {
        switch (p)
        {
            case GamePhase.Preparation:
                Time.timeScale = 1f;
                if (globalVolume != null)
                    globalVolume.weight = volumeDefaultWeight;
                if (popUpRoot != null)
                    popUpRoot.SetActive(false);
                if (popUpText != null)
                    popUpText.gameObject.SetActive(false);
                if (player != null)
                    player.enabled = true;
                if (bgm != null)
                    bgm.UnPause();

                // trigger dialogue
                DialogueStageManager.Instance.TriggerDialogue();
                break;

            case GamePhase.Playing:
                Time.timeScale = 1f;
                if (player != null)
                    player.enabled = true;
                if (bgm != null)
                    bgm.UnPause();
                break;

            case GamePhase.Paused:
                Time.timeScale = 0f;
                if (bgm != null)
                    bgm.Pause();
                break;

            case GamePhase.Dead:
                if (player != null)
                    player.enabled = false;
                if (globalVolume != null)
                    globalVolume.weight = volumeDefaultWeight;
                Time.timeScale = 0f;
                if (bgm != null)
                    bgm.Pause();
                if (popUpRoot != null)
                    popUpRoot.SetActive(true);
                if (popUpText != null)
                {
                    popUpText.gameObject.SetActive(true);
                    popUpText.text = "死亡了\n点击按钮重新开始";
                }
                if (restartButton != null)
                    restartButton.Select();
                break;
        }
    }

    void BindRestartButton()
    {
        if (restartButton == null) return;

        // 避免场景重载后重复绑定
        restartButton.onClick.RemoveListener(RestartLevel);
        restartButton.onClick.AddListener(RestartLevel);
    }

    void ActivateDangerVolume()
    {
        if (globalVolume == null) return;
        globalVolume.weight = volumeActiveWeight;
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;

        // 重新加载当前场景（关卡内障碍生成器也会一并重置）
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
