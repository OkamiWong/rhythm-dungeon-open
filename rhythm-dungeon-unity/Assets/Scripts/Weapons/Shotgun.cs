using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : RangeWeapon
{
    public bool isHigherOne;
    public bool isDamageFixed = false;
    public int fixedDamage;

    public override void Initialize(Stats shooter)
    {
        base.Initialize(shooter);

        if (isHigherOne)
        {
            if (shooterStats.verticalPositionState == 1)
                FaceTowards(new Vector3(0f, 0f, 1f));
            else FaceTowards(new Vector3(0f, Stats.verticalTranslation, 1f));
        }
        else
        {
            if (shooterStats.verticalPositionState == -1)
                FaceTowards(new Vector3(0f, 0f, 1f));
            else FaceTowards(new Vector3(0f, -Stats.verticalTranslation, 1f));
        }

        if (isDamageFixed)
        {
            damageRange = new DoubleInt(fixedDamage, fixedDamage);
            damageRate = 1f;
        }
    }

    private void Update()
    {
        if (shooterStats == null || shooterStats.gameObject == null)
            return;
        bool passMid = shooterStats.gameObject.tag == "Player" ? (transform.position.x > 0) : (transform.position.x < 0);
        if (passMid) transform.up = shooterStats.gameObject.tag == "Player" ? Vector2.right : Vector2.left;
    }
}
