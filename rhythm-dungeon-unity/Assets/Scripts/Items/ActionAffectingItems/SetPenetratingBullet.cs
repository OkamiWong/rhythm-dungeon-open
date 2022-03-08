using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPenetratingBullet : ActionAffectingItem
{
    [Range(0f, 1f)]
    public float damageScaler = 0.5f;

    public bool canPenetrateProtector, canPenetrateEnemy;

    private void Awake()
    {
        ItemType = Type.Attack;
    }

    public override void OnAdded(Stats owner)
    {
        ApplyTo(owner.mainBullet.GetComponent<RangeWeapon>());
        if (owner is PlayerStats)
            ApplyTo(PlayerController.Instance.bonusAttackBulletPrefab.GetComponent<RangeWeapon>());
    }

    void ApplyTo(RangeWeapon weapon)
    {
        weapon.canPenetrateEnemy |= canPenetrateEnemy;
        weapon.canPenetrateProtector |= canPenetrateProtector;

        if (canPenetrateEnemy)
            weapon.penetratDamageScaler += (0.5f - weapon.penetratDamageScaler) * damageScaler;
        if (canPenetrateProtector)
            weapon.penetratProtectorDamageScaler += (0.5f - weapon.penetratProtectorDamageScaler) * damageScaler;
    }
}
