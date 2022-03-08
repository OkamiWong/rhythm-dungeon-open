using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveAdditiveBullet : ActionAffectingItem
{
    public GameObject[] additiveBulletPrefabs;

    void Awake()
    {
        ItemType = Type.Attack;
    }

    public override void OnAdded(Stats owner)
    {
        foreach (GameObject bullet in additiveBulletPrefabs)
            owner.additiveBullets.Add(bullet);
    }
}
