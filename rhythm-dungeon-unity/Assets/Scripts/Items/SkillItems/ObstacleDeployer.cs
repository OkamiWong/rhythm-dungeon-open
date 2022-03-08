using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDeployer : SkillItem
{
    public GameObject obstaclePrefab;
    private GameObject obstacle;

    public override void Apply()
    {
        if (Effecting) return;
        if (CooldownCount < CooldownRounds) return;
        Effecting = true;

        obstacle = Instantiate(obstaclePrefab);
        float yPos = userStats.transform.position.y;
        float xPos = obstacle.transform.position.x;
        obstacle.transform.position = new Vector2(xPos, yPos);
    }

    public override void OnInvalid()
    {
        if (!Effecting) return;
        Effecting = false;

        Destroy(obstacle);
        obstacle = null;
    }
}
