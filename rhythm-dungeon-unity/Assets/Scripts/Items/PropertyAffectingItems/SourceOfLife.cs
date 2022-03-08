using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SourceOfLife : PropertyAffectingItem
{
    public float recoverAmount = 3;

    void Start()
    {
        NotificationCenter.Instance.AddEventHandler("TwoBarPassedInCombat", Recover);
    }

    void Recover(object sender, EventArgs e)
    {
        userStats.AddingHealth(recoverAmount);
    }

    void OnDestroy()
    {
        NotificationCenter.Instance.RemoveEventHandler("TwoBarPassedInCombat", Recover);
    }
}
