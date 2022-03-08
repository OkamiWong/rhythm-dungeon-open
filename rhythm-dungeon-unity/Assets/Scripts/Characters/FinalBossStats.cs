using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossStats : NormalCharacterStats
{
    public GameObject[] rawPossibleItems;

    public int minItemN, maxItemN;

    protected override void Start()
    {
        base.Start();

        var n = Random.Range(minItemN, maxItemN + 1);
        var possibleItems = new List<GameObject>(rawPossibleItems);
        var shuffledItems = new List<GameObject>();
        while (n > 0)
        {
            n--;
            var item = possibleItems[Random.Range(0, possibleItems.Count)];
            shuffledItems.Add(item);
            possibleItems.Remove(item);
        }
        foreach (GameObject item in shuffledItems)
        {
            if (item.GetComponent<ActionAffectingItem>() != null)
                AddActionAffectingItem(item);
            else if (item.GetComponent<PropertyAffectingItem>() != null)
                AddPropertyAffectingItem(item);
            else
            {
                if (SkillItems[0] == null)
                    AddSkillItem(item, 0);
                else AddSkillItem(item, 1);
            }
        }
    }
}
