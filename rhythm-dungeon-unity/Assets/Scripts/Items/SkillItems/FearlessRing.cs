using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearlessRing : SkillItem
{
    public int fearlessRounds = 5;
    public int damageEachTime = 10;
    public int deltaStrength = 50;

    [Header("Enemy Inspirer's Skill")]
    public bool isEnemyInspirer = false;

    public GameObject skillEffectPrefab;

    public override void Apply()
    {
        if (Effecting) return;
        if (CooldownCount < CooldownRounds) return;
        Effecting = true;

        if (isEnemyInspirer)
        {
            //special item for enemy inspirer
            if (EnemyManager.CurrentEnemyControllers.Count <= 1) return;
            EnemyStats stats; int cnt = 0;
            do //we don't want to inspire self
            {
                int index = Random.Range(0, EnemyManager.CurrentEnemyControllers.Count - 1);
                stats = EnemyManager.CurrentEnemyControllers[index].GetComponent<EnemyStats>();
            } while (stats == userStats && cnt++ < 5);
            StartCoroutine(EnemyInspire(stats));

        }
        else
        {
            //normal item
            userStats.effects.Add(FearlessEffect.Create(fearlessRounds, userStats, damageEachTime, userStats.Strength / 2));
        }

    }

    IEnumerator EnemyInspire(EnemyStats stats)
    {
        var skillEffect = Instantiate(skillEffectPrefab);
        skillEffect.GetComponent<SkillEffect>().Initialize(
            userStats.gameObject.transform.position,
            stats,
            GameManager.Instance.secondPerHalfBeat * 5f
        );
        yield return new WaitForSeconds(GameManager.Instance.secondPerHalfBeat * 5f);
        stats.effects.Add(FearlessEffect.Create(fearlessRounds, stats, damageEachTime, deltaStrength));
        yield break;
    }
}
