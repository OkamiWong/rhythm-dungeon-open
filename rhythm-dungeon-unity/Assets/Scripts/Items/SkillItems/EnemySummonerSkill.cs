using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySummonerSkill : SkillItem
{
    public GameObject enemyPrefab;
    public GameObject skillEffectPrefab;

    int? verticalPosition, horizontalPosition;

    protected virtual int? DetermineVerticalPosition()
    {
        List<int> possibleVerticalPositions = new List<int>();
        possibleVerticalPositions.Add(-1);
        possibleVerticalPositions.Add(0);
        possibleVerticalPositions.Add(1);
        foreach (EnemyController enemyController in EnemyManager.CurrentEnemyControllers)
            if (enemyController.enemyStats.enemyHorizontalPositionState == 0)
                possibleVerticalPositions.Remove(enemyController.enemyStats.verticalPositionState);
        if (possibleVerticalPositions.Count == 0)
            return null;
        return possibleVerticalPositions[Random.Range(0, possibleVerticalPositions.Count)];
    }

    protected virtual int? DetermineHorizontalPosition()
    {
        return 0;
    }

    public override void Apply()
    {
        if (Effecting) return;
        if (CooldownCount < CooldownRounds) return;

        verticalPosition = DetermineVerticalPosition();
        horizontalPosition = DetermineHorizontalPosition();
        if (verticalPosition == null || horizontalPosition == null) return;

        var skillEffect = Instantiate(skillEffectPrefab);
        skillEffect.GetComponent<SkillEffect>().Initialize(
            userStats.gameObject.transform.position,
            Vector3.up * (int)verticalPosition * Stats.verticalTranslation + Vector3.right * Stats.enemyX[(int)horizontalPosition],
            GameManager.Instance.secondPerHalfBeat * 5f
            );
        StartCoroutine(ApplyAfterTime(GameManager.Instance.secondPerHalfBeat * 5f));
    }

    IEnumerator ApplyAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);

        Effecting = true;
        EnemyManager.Instance.SummonEnemy(enemyPrefab, (int)verticalPosition, (int)horizontalPosition);
    }
}
