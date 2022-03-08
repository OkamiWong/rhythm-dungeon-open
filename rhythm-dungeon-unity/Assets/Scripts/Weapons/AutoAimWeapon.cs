using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAimWeapon : RangeWeapon
{
    //the speed when flying up
    public float firstSpeed = 0.5f;
    //the speed when flying toward target
    public float secondSpeed = 1f;

    bool autoAimed = false;

    protected override void Start()
    {
        NotificationCenter.Instance.AddEventHandler("AllEnemyKilled", OnAllEnemyKilled);
        StartCoroutine(AutoAimWeaponMove());
    }

    IEnumerator AutoAimWeaponMove()
    {
        //change face direction for flying up
        FaceTowards(transform.position + Vector3.up +
            ((shooterStats.gameObject.tag == "Player") ? Vector3.right : Vector3.left));
        while (true)
        {
            if (IsTimeToAutoAim()) AutoAim();

            if (!autoAimed)
                transform.Translate(Vector2.up * firstSpeed * Time.fixedDeltaTime);
            else
                transform.Translate(Vector2.up * secondSpeed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
    }

    bool IsTimeToAutoAim()
    {
        if (autoAimed)
            return false;
        if (shooterStats.gameObject.tag == "Player")
            return transform.position.x > shooterStats.transform.position.x * 0.5f;
        else
            return transform.position.x < shooterStats.transform.position.x * 0.5f;
    }

    //face to target
    void AutoAim()
    {
        autoAimed = true;
        if (shooterStats.gameObject.tag == "Player")
        {
            if (EnemyManager.CurrentEnemyControllers.Count > 0)
                FaceTowards(EnemyManager.CurrentEnemyControllers[0].transform.position);
        }
        else
        {
            FaceTowards(PlayerController.Instance.transform.position);
        }
    }
}
