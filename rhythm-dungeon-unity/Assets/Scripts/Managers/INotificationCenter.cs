using System;
using System.Collections;
using System.Collections.Generic;

// NotificationCenter的抽象基类
public abstract class INotificationCenter
{
    protected Dictionary<string, EventHandler> eventTable;

    protected INotificationCenter()
    {
        eventTable = new Dictionary<string, EventHandler>();
    }

    // 将名字为name，发送者为sender，参数为e的消息发送出去
    public void PostNotification(string name)
    {
        PostNotification(name, null, EventArgs.Empty);
    }
    public void PostNotification(string name, object sender)
    {
        PostNotification(name, sender, EventArgs.Empty);
    }
    public void PostNotification(string name, object sender, EventArgs e)
    {
        if (eventTable.ContainsKey(name) && eventTable[name] != null)
			eventTable[name](sender, e);
    }

    // 添加或者移除一个回调函数
    public void AddEventHandler(string name, EventHandler handler)
    {
		if (!eventTable.ContainsKey(name))
			eventTable.Add(name, handler);
		else
			eventTable[name] += handler;
    }
    public void RemoveEventHandler(string name, EventHandler handler)
    {
		if (eventTable.ContainsKey(name))
			eventTable[name] -= handler;
    }
}