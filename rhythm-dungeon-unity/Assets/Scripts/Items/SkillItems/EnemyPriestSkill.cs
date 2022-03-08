using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPriestSkill : SkillItem
{
    public bool recoverTheMostNeededOne = true;
    public GameObject skillEffectPrefab;

    public override void Apply()
    {
        if (Effecting) return;
        if (CooldownCount < CooldownRounds) return;
        Effecting = true;

        EnemyController wh = EnemyManager.CurrentEnemyControllers[0];
        if (recoverTheMostNeededOne)
        {
            float rate = 1.01f;
            foreach (var ec in EnemyManager.CurrentEnemyControllers)
            {
                var stats = ec.GetComponent<EnemyStats>();
                var r = (float)stats.CurrentHealth / (float)stats.MaxHealth;
                if (r < rate) { rate = r; wh = ec; }
            }
        }
        else
        {
            int index = Random.Range(0, EnemyManager.CurrentEnemyControllers.Count - 1);
            wh = EnemyManager.CurrentEnemyControllers[index];
        }

        StartCoroutine(Heal(wh.GetComponent<EnemyStats>()));
    }

    IEnumerator Heal(EnemyStats target)
    {
        var skillEffect = Instantiate(skillEffectPrefab);
        skillEffect.GetComponent<SkillEffect>().Initialize(
            userStats.gameObject.transform.position,
            target,
            GameManager.Instance.secondPerHalfBeat * 5f
        );
        yield return new WaitForSeconds(GameManager.Instance.secondPerHalfBeat * 5f);
        target.AddingHealth(userStats.Strength);
    }
}
