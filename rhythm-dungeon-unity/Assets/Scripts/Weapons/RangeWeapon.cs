using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangeWeapon : MonoBehaviour
{
    [Header("Parameters")]
    public float damageRate = 1f;
    public bool enableItemAffects = true;

    [HideInInspector]
    public DoubleInt damageRange;

    [HideInInspector]
    //[Header("Enemy Range Weapon")]
    public float distancePerBeat = 1.2f / 9.0f;

    [Header("Player Range Weapon")]
    public float moveSpeed = 0.7f;

    protected Stats shooterStats;

    [HideInInspector]
    public string selfTag, targetTag;

    //about penetrat
    [HideInInspector]
    public bool canPenetrateProtector = false;
    [HideInInspector]
    public bool canPenetrateEnemy = false;
    [HideInInspector]
    public float penetratDamageScaler = 0f;

    public float penetratProtectorDamageScaler = 0f;

    public virtual void Initialize(Stats shooter)
    {
        shooterStats = shooter;

        selfTag = shooterStats.gameObject.tag;
        if (selfTag == "Player")
            targetTag = "Enemy";
        else targetTag = "Player";

        if (selfTag == "Enemy")
        {
            distancePerBeat =
                (Stats.enemyX[shooterStats.enemyHorizontalPositionState] - Stats.playerX) / 9f;
        }

        transform.position = shooterStats.transform.position; //Set weapon postion

        UpdateDamageRate();

        if (selfTag == "Player")
            FaceTowards(transform.position + Vector3.right);
        else
            FaceTowards(transform.position + Vector3.left);
    }

    protected virtual void UpdateDamageRate() { }

    protected void FaceTowards(Vector3 target)
    {
        Vector2 dir = new Vector2(target.x - transform.position.x, target.y - transform.position.y);
        transform.up = dir;
    }

    protected virtual void Start()
    {
        NotificationCenter.Instance.AddEventHandler("AllEnemyKilled", OnAllEnemyKilled);

        StartCoroutine(DestoryAfterTime(10f));
        if (selfTag == "Enemy")
            StartCoroutine(EnemyWeaponMove());
        else StartCoroutine(PlayerWeaponMove());
    }

    IEnumerator EnemyWeaponMove()
    {
        while (true)
        {
            double moveTime = GameManager.Instance.secondPerHalfBeat * 0.5;
            int steps = (int)(moveTime / Time.fixedDeltaTime);
            float distancePerStep = distancePerBeat / steps;

            while (steps-- > 0)
            {
                transform.Translate(Vector2.up * distancePerStep);
                yield return new WaitForFixedUpdate();
            }

            float timeConsume = Time.fixedDeltaTime * steps;
            float idleTime = GameManager.Instance.secondPerHalfBeat * 2 - timeConsume;
            yield return new WaitForSeconds(idleTime);
        }
    }

    IEnumerator PlayerWeaponMove()
    {
        while (true)
        {
            transform.Translate(Vector2.up * moveSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Obstacle")
        {
            SelfDestroy();
        }
        else if (collider.gameObject.tag == "Protector")
        {
            if (collider.GetComponent<Protector>().selfTag != selfTag)
                OnPenetratingProtector();
        }
        else if (collider.gameObject.tag == targetTag)
        {
            var targetStats = collider.gameObject.GetComponent<Stats>();
            targetStats.TakingDamage(damageRange.Random() * damageRate);

            if (shooterStats) //if shooter alive, then apply item effects
            {
                if (enableItemAffects)
                    foreach (OnHittingTarget AfterHit in shooterStats.AfterHitFunctions)
                        AfterHit(targetStats);
                foreach (OnBeingHitted AfterHitted in targetStats.AfterHittedFunctions)
                    AfterHitted(shooterStats);
            }

            OnPenetratingEnemy();
        }
    }

    void OnPenetratingProtector()
    {
        if (canPenetrateProtector)
        {
            damageRate *= penetratProtectorDamageScaler;
            transform.localScale *= Mathf.Lerp(penetratProtectorDamageScaler, 1f, 0.5f);
        }
        else SelfDestroy();
    }

    void OnPenetratingEnemy()
    {
        if (canPenetrateEnemy)
        {
            damageRate *= penetratDamageScaler;
            transform.localScale *= Mathf.Lerp(penetratDamageScaler, 1f, 0.5f);
        }
        else SelfDestroy();
    }


    //because the gameobject must have a render to enable the OnBecomeInvisible
    //I change it back to use time to destory the bullet after it flew out of screen
    IEnumerator DestoryAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        SelfDestroy();
    }

    //when you beat all enemies, all flying buttles should disappear.
    protected void OnAllEnemyKilled(object sender, EventArgs e)
    {
        SelfDestroy();
    }

    //Destroy method
    //may be called more than one times beacuse different reasons
    //check whether gameObject is null first
    public virtual void SelfDestroy()
    {
        if (gameObject == null) return;
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        NotificationCenter.Instance.RemoveEventHandler("AllEnemyKilled", OnAllEnemyKilled);
    }
}