using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAction
{
    Idle, MoveUp, MoveDown, Charge, Attack, UseItem_0, UseItem_1
}

//The component to handle all the behaviors of enemies.
public class EnemyController : MonoBehaviour
{
    Combat enemyCombat;

    [HideInInspector]
    public EnemyStats enemyStats;

    [HideInInspector]
    public EnemyAI enemyAI;

    private void Awake()
    {
        enemyCombat = gameObject.AddComponent<Combat>();
        enemyStats = GetComponent<EnemyStats>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void OnDestroy()
    {
        EnemyManager.CurrentEnemyControllers.Remove(this);
    }

    public void DeployAction(EnemyAction action)
    {
        if (enemyStats.FreezeCount > 0) return;

        if (enemyStats == null || gameObject == null)
        {
            EnemyManager.EnemyControllersToRemove.Add(this);
            return;
        }

        switch (action)
        {
            case EnemyAction.Idle: break;
            case EnemyAction.MoveUp: Move(true); break;
            case EnemyAction.MoveDown: Move(false); break;
            case EnemyAction.Charge: Charge(); break;
            case EnemyAction.Attack: Attack(); break;
            case EnemyAction.UseItem_0: UseItem(0); break;
            case EnemyAction.UseItem_1: UseItem(1); break;
            default: break;
        }

        var eventParams = new Dictionary<string, string>();
        eventParams["action_type"] = '"' + action.ToString() + '"';
        eventParams["index"] = enemyStats.indexInGame.ToString();
        DataCollector.Instance.CodeAndSendEvent("enemy_act", eventParams);
    }

    public void Attack()
    {
        bool isCritical = (UnityEngine.Random.Range(0, 1000) < enemyStats.Luck);
        enemyCombat.Attack(isCritical);
        enemyStats.ClearCharge();
    }

    public void Move(bool isUpward)
    {
        if (!enemyStats.ChangePositionState(isUpward)) return;
        StartCoroutine(MoveCoroutine(isUpward ? 1 : -1));
    }

    IEnumerator MoveCoroutine(int directionalFactor)
    {
        int totalStep = (int)(GameManager.Instance.secondPerHalfBeat / Time.fixedDeltaTime);
        float stepLength = Stats.verticalTranslation / (float)totalStep;

        for (int i = 0; i < totalStep; ++i)
        {
            transform.position += new Vector3(0f, directionalFactor * stepLength, 0f);
            yield return new WaitForFixedUpdate();
        }
    }

    void Charge()
    {
        enemyStats.Charge();
    }

    void UseItem(int which)
    {
        var items = enemyStats.SkillItems;
        if (which >= items.Length || items[which] == null) return;
        items[which].Apply();
    }

    public void DeployAIAction()
    {
        DeployAction(enemyAI.GetAction());
    }
}