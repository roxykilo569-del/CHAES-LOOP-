using UnityEngine;

/// <summary>
/// 障碍触碰：第一次进入危险状态，第二次触碰则死亡并在复活点重生。
/// 需在场景中放置复活点 Transform，并指定玩家引用（或挂有 PlayerController 的物体）。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("玩家与复活")]
    [Tooltip("复活位置（出生点）。不填则使用玩家初始位置。")]
    public Transform respawnPoint;

    [Tooltip("不填则自动在场景中查找 PlayerController。")]
    public PlayerController player;

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

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        if (player != null && respawnPoint != null)
            player.SetSpawnPoint(respawnPoint.position);
    }

    /// <summary>
    /// 由 PlayerController 在碰到 Tag 为 Obstacle 的物体时调用。
    /// </summary>
    public void OnPlayerHitObstacle()
    {
        if (player == null) return;

        if (!inDanger)
        {
            inDanger = true;
            return;
        }

        inDanger = false;
        Vector3 pos = respawnPoint != null ? respawnPoint.position : player.GetSpawnPoint();
        player.RespawnAt(pos);
    }

    /// <summary>
    /// 需要时可在别处重置危险状态（例如过关、读档）。
    /// </summary>
    public void ClearDanger()
    {
        inDanger = false;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
