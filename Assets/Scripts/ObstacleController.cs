using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("Spawn")]
    [Tooltip("可生成的障碍 Prefab（每次随机挑一个）。")]
    public GameObject[] obstaclePrefabs;

    [Tooltip("生成点（不填就用当前物体的位置）。")]
    public Transform spawnPoint;

    [Min(0.05f)]
    [Tooltip("基础生成间隔（秒）。")]
    public float spawnInterval = 1.5f;

    [Tooltip("在基础间隔上额外随机加/减（秒）。例如 0.3 表示 1.2~1.8 之间。")]
    public float intervalJitter = 0.0f;

    [Tooltip("生成 Y 轴随机范围（适合有高低障碍/地面起伏的关卡）。不需要就设成 (0,0)。")]
    public Vector2 spawnYRange = new Vector2(0f, 0f);

    [Header("Obstacle Move (applied to Obstacle script if present)")]
    [Min(0f)]
    public float obstacleMoveSpeed = 5f;

    public float destroyX = -20f;

    Coroutine spawnRoutine;

    void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    IEnumerator SpawnLoop()
    {
        // 避免刚开始就贴脸生成：等一小段再开始（也可以把它设为 0）
        yield return new WaitForSeconds(0.2f);

        while (true)
        {
            TrySpawnOne();

            float jitter = intervalJitter <= 0f ? 0f : Random.Range(-intervalJitter, intervalJitter);
            float wait = Mathf.Max(0.05f, spawnInterval + jitter);
            yield return new WaitForSeconds(wait);
        }
    }

    void TrySpawnOne()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        if (prefab == null) return;

        Vector3 basePos = spawnPoint != null ? spawnPoint.position : transform.position;
        float yOffset = (spawnYRange.x == 0f && spawnYRange.y == 0f) ? 0f : Random.Range(spawnYRange.x, spawnYRange.y);
        Vector3 spawnPos = new Vector3(basePos.x, basePos.y + yOffset, basePos.z);

        GameObject go = Instantiate(prefab, spawnPos, prefab.transform.rotation);

        // 如果 prefab 上挂了 Obstacle 脚本，就把移动参数同步过去
        Obstacle obstacle = go.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            obstacle.moveSpeed = obstacleMoveSpeed;
            obstacle.destroyX = destroyX;
        }
    }
}
