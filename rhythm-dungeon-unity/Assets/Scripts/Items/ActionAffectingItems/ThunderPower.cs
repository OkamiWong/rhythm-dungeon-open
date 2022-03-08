using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderPower : ActionAffectingItem
{
    public GameObject thunderPrefab;

    [Range(0f, 1f)]
    public float possibility = 0.5f;
    public float damage = 50;

    public float showTime = 2f;

    void Awake()
    {
        ItemType = Type.Attack;
    }

    public override void OnAdded(Stats owner)
    {
        owner.AfterHitFunctions.Add(SummonThunder);
    }

    void SummonThunder(Stats target)
    {
        if (Random.Range(0f, 1f) < possibility)
        {
            AudioManager.Instance.PlayLightning();
            if (target.gameObject.tag == "Enemy")
            {
                foreach (EnemyController c in EnemyManager.CurrentEnemyControllers)
                {
                    var thunder = Instantiate(thunderPrefab).GetComponent<ThunderEffect>();
                    StartCoroutine(thunder.ThunderProcess(c.GetComponent<Stats>(), showTime, damage));
                }
            }
            else //target is player
            {
                var thunder = Instantiate(thunderPrefab).GetComponent<ThunderEffect>();
                StartCoroutine(thunder.ThunderProcess(PlayerStats.Instance, showTime, damage));
            }
        }
    }
}
