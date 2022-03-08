using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EvilBlood : PropertyAffectingItem
{
    int enemyKilled;
    void Start()
    {
        enemyKilled = 0;
        NotificationCenter.Instance.AddEventHandler("EnemyKilled", OnEnemyKilled);
    }

    void OnDestroy()
    {
        NotificationCenter.Instance.RemoveEventHandler("EnemyKilled", OnEnemyKilled);
    }

    void OnEnemyKilled(object sender, EventArgs e)
    {
        if (((EnemyStats)sender).isSummoned)
            return;
        enemyKilled += 1;
    }

    public override int CalculateDeltaMaxHealth()
    {
        return enemyKilled * 5;
    }
}
