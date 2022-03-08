using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineDeployer : SkillItem
{
    public GameObject landminePrefab;

	public List<Landmine> mines = new List<Landmine>();

    public override void Apply()
    {
        if (Effecting) return;
        if (CooldownCount < CooldownRounds) return;
        Effecting = true;

        int damage = userStats.Strength * 2;
		int cnt = userStats.tag == "Enemy" ? 1 : 3;

		for (int i = 0; i < cnt; i++)
		{
			//calculate pos
			float xpos = userStats.tag == "Enemy" ? Stats.playerX : Stats.enemyX[i];
			float ypos = userStats.verticalPositionState * Stats.verticalTranslation;

			bool flag = true;
			//if there exists a mine in that place, don't deploy a new one.
			foreach (Landmine existedMine in mines)
			{
				if (Mathf.Abs(existedMine.transform.position.x - xpos) < 0.001f && 
					Mathf.Abs(existedMine.transform.position.y - ypos) < 0.001f)
				{ flag = false; break; }
			}
			if (!flag) continue;

			Landmine mine = Instantiate(landminePrefab).GetComponent<Landmine>();
			//init weapon
			mine.damageRange = new DoubleInt(damage, damage);
			mine.Initialize(userStats);
			//set position
			mine.transform.position = new Vector2(xpos, ypos);
			//add to list
			mine.deployer = this;
			mines.Add(mine);
		}
    }
}
