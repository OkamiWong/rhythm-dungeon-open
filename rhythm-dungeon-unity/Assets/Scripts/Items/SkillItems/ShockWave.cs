using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : SkillItem
{
    public GameObject thunderPrefab;
    public float damageRate = 2f;
    public float showTime = 2f;

    public override void Apply()
    {
        if (Effecting) return;
        if (CooldownCount < CooldownRounds) return;
        Effecting = true;

        AudioManager.Instance.PlayLightning();

        List<Stats> list = GetOtherSideStatsList();
        foreach (Stats s in list)
        {
            var thunder = Instantiate(thunderPrefab).GetComponent<ThunderEffect>();
            StartCoroutine(thunder.ThunderProcess(s, showTime, userStats.Strength * damageRate));
        }
    }
}
