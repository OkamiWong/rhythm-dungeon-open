using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderEffect : MonoBehaviour
{
    public IEnumerator ThunderProcess(Stats stats, float showTime, float damage)
    {
        transform.position = stats.transform.position;
        transform.SetParent(stats.transform);
        Destroy(gameObject, showTime);
        yield return new WaitForSeconds(showTime / 2f);
        if (stats == null || stats.CurrentHealth <= 0) yield break;
        stats.TakingDamage(damage);
    }
}
