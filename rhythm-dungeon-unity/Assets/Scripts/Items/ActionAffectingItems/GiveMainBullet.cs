using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMainBullet : ActionAffectingItem
{
    public GameObject mainBulletPrefab;

    void Awake()
    {
        ItemType = Type.Attack;
    }

    public override void OnAdded(Stats owner)
    {
        owner.mainBullet = mainBulletPrefab;
    }
}
