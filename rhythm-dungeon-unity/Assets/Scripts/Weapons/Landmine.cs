using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : RangeWeapon
{
    [HideInInspector]
    public MineDeployer deployer;

    public GameObject executionPrefab;

    protected override void Start()
    {
        NotificationCenter.Instance.AddEventHandler("AllEnemyKilled", OnAllEnemyKilled);
        //correct the up direction
        transform.up = Vector3.up;
    }

    public override void Initialize(Stats shooter)
    {
        base.Initialize(shooter);
        if (shooter.tag == "Enemy")
            enableItemAffects = false;
        else enableItemAffects = true;
    }

    public override void SelfDestroy()
    {
        if (deployer) deployer.mines.Remove(this);

        if (EnemyManager.CurrentEnemyControllers.Count != 0)
        {
            var effect = Instantiate(executionPrefab);
            effect.transform.position = transform.position;
        }

        base.SelfDestroy();
    }
}
