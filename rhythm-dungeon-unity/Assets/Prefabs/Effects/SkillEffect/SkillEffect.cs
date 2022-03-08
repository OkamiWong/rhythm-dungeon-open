using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public GameObject movingEffect, endingEffect;
    Vector3 target, unitDirection;
    Stats targetStats = null;
    bool ended = false;
    float velocity;
    public void Initialize(Vector3 start, Vector3 end, float time)
    {
        transform.position = start;
        target = end;
        movingEffect.SetActive(true);
        endingEffect.SetActive(false);
        velocity = Vector3.Distance(start, end) / time;
        unitDirection = (end - start).normalized;
    }

    public void Initialize(Vector3 start, Stats end, float time)
    {
        Initialize(start, end.gameObject.transform.position, time);
        targetStats = end;
    }

    void FixedUpdate()
    {
        if (!ended)
        {
            var translation = Time.fixedDeltaTime * velocity;
            if (targetStats != null)
            {
                target = targetStats.gameObject.transform.position;
                unitDirection = (target - transform.position).normalized;
            }

            if (Vector3.Distance(transform.position, target) <= translation)
            {
                transform.position = target;
                ended = true;
                movingEffect.SetActive(false);
                endingEffect.SetActive(true);
            }
            else transform.Translate(unitDirection * translation);
        }
    }
}
