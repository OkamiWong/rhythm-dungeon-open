using UnityEngine;
using System;

// 在这里弄出多个INotificationCenter的子类
// 分别处理不同的消息转发，便于消息分组
public class NotificationCenter : INotificationCenter
{
    private static NotificationCenter _instance;

    private event EventHandler EnemyKilled;
    private event EventHandler AllEnemyKilled;
    private event EventHandler OnHalfBeat;
    private event EventHandler EnterGameState;
    private event EventHandler TwoBarPassedInCombat;

    private NotificationCenter() : base()
    {
        // 在这里添加需要分发的各种消息
        eventTable.Add("EnemyKilled", EnemyKilled);
        eventTable.Add("AllEnemyKilled", AllEnemyKilled);
        eventTable.Add("OnHalfBeat", OnHalfBeat);
        eventTable.Add("EnterGameState", EnterGameState);
        eventTable.Add("TwoBarPassedInCombat", TwoBarPassedInCombat);
    }

    public static NotificationCenter Instance
    {
        get
        {
            if (_instance == null)
                _instance = new NotificationCenter();
            return _instance;
        }
    }
}