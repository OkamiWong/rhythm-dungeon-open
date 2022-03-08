using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basic class of all enemy AI
public class EnemyAI: MonoBehaviour
{
	[HideInInspector]
	public EnemyStats enemyStats;

	protected virtual void Start()
	{
		enemyStats = GetComponent<EnemyStats>();
	}

	public virtual EnemyAction GetAction()
	{
		return EnemyAction.Idle;
	}

	public EnemyAction GetTrackPlayerMove()
	{
		int playerPos = PlayerStats.Instance.verticalPositionState;
		int enemyPos = enemyStats.verticalPositionState;
		if (playerPos > enemyPos) return EnemyAction.MoveUp;
		else if (playerPos < enemyPos) return EnemyAction.MoveDown;
		else return EnemyAction.Idle;
	}
}
