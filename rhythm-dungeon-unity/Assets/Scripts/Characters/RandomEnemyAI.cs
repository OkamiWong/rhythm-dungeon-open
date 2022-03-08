using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemyAI : EnemyAI
{
    [Header("Probabilities:")]
    public float idleProbability = 0.5f;
    public float moveProbabilityWhenFacingPlayer = 0.5f;
    public float moveProbabilityWhenNotFacingPlayer = 2f;
    public float chargeProbability = 1f;
    public float attackProbability = 1.5f;
    public float useItemProbability = 1f;
    public float aggressiveProbability = 0f;

    public override EnemyAction GetAction()
    {
        var items = enemyStats.SkillItems;
        bool flag0 = items[0] != null && items[0].CooldownCount == items[0].CooldownRounds;
        bool flag1 = items[1] != null && items[1].CooldownCount == items[1].CooldownRounds;

        bool canUp = enemyStats.IsMoveViable(true);
        bool canDown = enemyStats.IsMoveViable(false);

        var currentAttackProbability = attackProbability
            * Mathf.Min(1f, 0.4f + (float)PlayerStats.Instance.CurrentHealth / (float)PlayerStats.Instance.MaxHealth);

        var currentMoveProbability = (
            enemyStats.verticalPositionState == PlayerStats.Instance.verticalPositionState ?
            moveProbabilityWhenFacingPlayer :
            moveProbabilityWhenNotFacingPlayer
        );
        if (!canUp && !canDown) currentMoveProbability = 0;

        var currentChargeProbability = (
            Mathf.Abs(enemyStats.ChargeFactor - 1f) > Mathf.Epsilon ?
            0f : chargeProbability
        );

        var currentUseItemProbility = (flag0 || flag1) ? useItemProbability : 0f;

        float total =
            idleProbability +
            currentMoveProbability +
            currentChargeProbability +
            currentAttackProbability +
            currentUseItemProbility +
            aggressiveProbability;

        float randNum = Random.Range(0, total);

        if (randNum < idleProbability) return EnemyAction.Idle;
        else randNum -= idleProbability;

        if (randNum < currentMoveProbability && (canUp || canDown))
        {
            if (canUp && canDown)
                return Random.Range(0, 2) == 0 ?
                    EnemyAction.MoveUp :
                    EnemyAction.MoveDown;
            else if (canUp) return EnemyAction.MoveUp;
            else return EnemyAction.MoveDown;
        }
        randNum -= currentMoveProbability;

        if (randNum < currentChargeProbability) return EnemyAction.Charge;
        randNum -= currentChargeProbability;

        if (randNum < currentAttackProbability) return EnemyAction.Attack;
        randNum -= currentAttackProbability;
        
        if (randNum < useItemProbability && (flag0 || flag1))
        {
            if (flag0 && flag1)
                return Random.Range(0, 2) == 0 ? 
                    EnemyAction.UseItem_0 : 
                    EnemyAction.UseItem_1;
            else if (flag0) return EnemyAction.UseItem_0;
            else return EnemyAction.UseItem_1;
        }
        randNum -= currentUseItemProbility;

        if (randNum < aggressiveProbability)
        {
            var move = GetTrackPlayerMove();

            if (move == EnemyAction.Idle) return EnemyAction.Attack;

            var isUp = move == EnemyAction.MoveUp;
            if (enemyStats.IsMoveViable(isUp)) return move;

            if (currentChargeProbability > 0) return EnemyAction.Charge;
            else return EnemyAction.Attack;
        }
        randNum -= aggressiveProbability;

        //this won't be used. just make sure this funcion always have a return.
        return EnemyAction.Idle;
    }
}
