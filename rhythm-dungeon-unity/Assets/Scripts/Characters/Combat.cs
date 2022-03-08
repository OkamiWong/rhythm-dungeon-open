using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The component to deploy attack.
public class Combat : MonoBehaviour
{
    Stats stats;

    private void Start()
    {
        stats = GetComponent<Stats>();
    }

    //The method to attack immediately.
    public void Attack(bool isCritical)
    {
        int damage = (int)(stats.Strength * stats.ChargeFactor);
        float higherDelay = 0f, lowerDelay = 0f, midDelay = 0f;
        float interval = GameManager.Instance.secondPerHalfBeat;

        if (stats.mainBullet != null)
        {
            Shoot(stats.mainBullet, damage);
            midDelay += interval;
            if (isCritical)
            {
                bool isPlayer = tag == "Player";
                UIManager.Instance.ShowCritical(isPlayer);
                if (!isPlayer) midDelay += interval * 2;
                StartCoroutine(ShootAfterSecond(midDelay, stats.mainBullet, damage));
                midDelay += interval;
            }
        }

        foreach (GameObject bullet in stats.additiveBullets)
        {
            var shotgun = bullet.GetComponent<Shotgun>();
            if (shotgun)
            {
                if (shotgun.isHigherOne)
                {
                    StartCoroutine(ShootAfterSecond(higherDelay, bullet, damage));
                    higherDelay += interval;
                }
                else
                {
                    StartCoroutine(ShootAfterSecond(lowerDelay, bullet, damage));
                    lowerDelay += interval;
                }
            }
            else
            {
                StartCoroutine(ShootAfterSecond(midDelay, bullet, damage));
                midDelay += interval;
            }
        }
    }

    IEnumerator ShootAfterSecond(float delay, GameObject bulletPrefab, int damage)
    {
        yield return new WaitForSeconds(delay);
        Shoot(bulletPrefab, damage);
    }

    public void Shoot(GameObject bulletPrefab, int damage)
    {
        var bullet = Instantiate(bulletPrefab);
        var rangeWeapon = bullet.GetComponent<RangeWeapon>();
        rangeWeapon.damageRange = new DoubleInt(damage, damage);
        rangeWeapon.Initialize(stats);
        if (stats.ChargeFactor > 1.01f) //bullet with charge is larger
            bullet.transform.localScale *= stats.ChargeFactor / 1.5f;
    }
}