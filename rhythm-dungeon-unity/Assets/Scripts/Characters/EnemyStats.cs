using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The component to handle the stats of enemies.
public class EnemyStats : Stats
{
    //Cached components
    public Image hpBar;

    /*
    (0,1) (1,1) (2,1)
    (0,0) (1,0) (2,0)
    (0,-1) (1,-1) (2,-1)
    */
    [Header("Possible Initial Position")]
    public bool[] leftColumn = { true, true, true };
    public bool[] midColumn = { true, true, true };
    public bool[] rightColumn = { true, true, true };

    [HideInInspector]
    public bool isSummoned = false;

    [HideInInspector]
    public bool isPropertyInitialized = false;

    [HideInInspector]
    public int indexInGame;// for tagging events

    public KeyValuePair<int, int> GetInitialPositionState()
    {
        //There is no Pair<T,T> in C#, so KeyValuePair<T,T> is used here.
        //Key: Horizontal Position, Value: Vertical Position
        List<KeyValuePair<int, int>> possibleLocations = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < 3; ++i)
        {
            if (leftColumn[i])
                possibleLocations.Add(new KeyValuePair<int, int>(0, i - 1));
            if (midColumn[i])
                possibleLocations.Add(new KeyValuePair<int, int>(1, i - 1));
            if (rightColumn[i])
                possibleLocations.Add(new KeyValuePair<int, int>(2, i - 1));
        }
        foreach (EnemyController enemyController in EnemyManager.CurrentEnemyControllers)
        {
            var enemyPosition = new KeyValuePair<int, int>(
                    enemyController.enemyStats.enemyHorizontalPositionState,
                    enemyController.enemyStats.verticalPositionState
                );
            if (possibleLocations.Contains(enemyPosition))
                possibleLocations.Remove(enemyPosition);
        }

        if (possibleLocations.Count == 0)
            return new KeyValuePair<int, int>(233, 233); //where 233 denotes no possible position
        return possibleLocations[Random.Range(0, possibleLocations.Count)];
    }

    public override bool IsMoveViable(bool isUpward)
    {
        if (!base.IsMoveViable(isUpward)) return false;
        return IsThereAnyEnemyBelowOrAbove(isUpward ? 1 : -1);
    }

    bool IsThereAnyEnemyBelowOrAbove(int delta)
    {
        foreach (EnemyController enemyController in EnemyManager.CurrentEnemyControllers)
        {
            if (enemyController.enemyStats.enemyHorizontalPositionState == enemyHorizontalPositionState)
                if (enemyController.enemyStats.verticalPositionState == verticalPositionState + delta)
                    return false;
        }
        return true;
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeProperty();
    }

    protected override void Start()
    {
        base.Start();
        //fill hp at start
        CurrentHealth = MaxHealth;
        hpBar.fillAmount = 1f;
    }

    //Property intiallizer can be overriden to initialize the stats in other ways
    //rather than pre-assigning all the properties.
    protected virtual void InitializeProperty()
    {
        isPropertyInitialized = true;
    }

    //Сaused by taking damage
    public override void TakingDamage(float damage, bool clearChargeFlag = true)
    {
        if (!this || !gameObject) return;
        if (damage < Mathf.Epsilon) return;
        base.TakingDamage(damage, clearChargeFlag);

        hpBar.fillAmount = (float)CurrentHealth / (float)MaxHealth; //Update hp bar
        AudioManager.Instance.Play(AudioManager.Instance.soundEffect, AudioManager.Instance.aiDamage, false); //play damage sound

        if (CurrentHealth <= 0) //if HP < 0, Death
        {
            Death();
        }
    }

    public override void AddingHealth(float recoverAmount)
    {
        base.AddingHealth(recoverAmount);

        hpBar.fillAmount = (float)CurrentHealth / MaxHealth; //Update hp bar
    }

    public override void Death()
    {
        if (!this || !gameObject) return;

        var eventParams = new Dictionary<string, string>();
        eventParams["index"] = indexInGame.ToString();
        DataCollector.Instance.CodeAndSendEvent("enemy_die", eventParams);

        base.Death();
        EnemyManager.EnemyControllersToRemove.Add(GetComponent<EnemyController>());
        NotificationCenter.Instance.PostNotification("EnemyKilled", this);

        Destroy(gameObject);
    }
}