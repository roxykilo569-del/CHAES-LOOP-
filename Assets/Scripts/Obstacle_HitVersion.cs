using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_HitVersion : MonoBehaviour
{
    public bool isInSafeZone;
    public bool isAfterSafeZone;
    public bool isJudged;
    public bool isHit;

    public Vector2 targetPoint;

    public BoxCollider2D safeZone;
    public BoxCollider2D dangerousZone;

    [Min(0f)]
    public float moveSpeed = 5f;

    [Tooltip("当障碍物的 x 坐标小于该值时自动销毁。")]
    public float destroyX = -20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.Phase != GamePhase.Playing)
            return;

        transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime), Space.World);

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }

        if (isJudged)
            return;

        if (isAfterSafeZone)
        {
            // what is going on?
            // hit
            if (isHit)
            {

            }
            else
            {
                GameManager.Instance.OnPlayerHitObstacle();
            }
            isInSafeZone = false;
            isAfterSafeZone = false;
            isJudged = true;

            return;
        }
        if (isInSafeZone)
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                isHit = true;
            }
        }
        // SafeTriggerZone
        // detect if player hit Z keyboard

        List<Collider2D> result = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.NameToLayer("Player");
        safeZone.OverlapCollider(filter, result);

        for(int i =0; i < result.Count; i++)
        {
            Collider2D collider = result[i];
            if(collider != null)
            {
                if(collider.gameObject.layer == filter.layerMask)
                {
                    isInSafeZone = true;
                }
            }
        }

        dangerousZone.OverlapCollider(filter, result);
        for (int i = 0; i < result.Count; i++)
        {
            Collider2D collider = result[i];
            if (collider != null)
            {
                if (collider.gameObject.layer == filter.layerMask && isInSafeZone)
                {
                    isAfterSafeZone = true;
                }
            }
        }


        
    }


}
