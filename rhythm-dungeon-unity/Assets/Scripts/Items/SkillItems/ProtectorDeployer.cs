using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectorDeployer : SkillItem
{
    public GameObject protectorPrefab;
    private GameObject protector;

    public int protectorHealth = 1;

    public override void Apply()
    {
        if (Effecting) return;
        if (CooldownCount < CooldownRounds) return;
        Effecting = true;

        protector = Instantiate(protectorPrefab);
        protector.transform.position = userStats.transform.position;
        protector.transform.parent = userStats.transform;
        var component = protector.GetComponent<Protector>();
        component.health = protectorHealth;
        component.item = this;
    }

    public override void OnInvalid()
    {
        if (!Effecting) return;
        Effecting = false;

        if (protector != null)
            Destroy(protector);
        protector = null;
    }
}
