using System;
using System.Collections.Generic;

/// <summary>
/// 简单的事件系统
/// </summary>
public class SimpleEventCenter
{
    private Dictionary<XOGameEventEnum, Action<SimpleEventParam>> center =
        new Dictionary<XOGameEventEnum, Action<SimpleEventParam>>();

    public void SendEvent(XOGameEventEnum eventType, SimpleEventParam param)
    {
        if (center.TryGetValue(eventType, out var curAct))
        {
            curAct?.Invoke(param);
        }
    }

    public void Register(XOGameEventEnum eventType, Action<SimpleEventParam> callback)
    {
        if (center.TryGetValue(eventType, out var curAct))
        {
            curAct += callback;
            center[eventType] = curAct;
        }
        else
        {
            center.Add(eventType, callback);
        }
    }

    public void UnRegister(XOGameEventEnum eventType, Action<SimpleEventParam> callback)
    {
        if (center.TryGetValue(eventType, out var curAct))
        {
            curAct -= callback;
            center[eventType] = curAct;
        }
    }
}

public enum XOGameEventEnum
{
    GameOver,
    PlayerDoStep,
    PlayerUnDoStep,
    SwitchHand
}

public class SimpleEventParam
{
}

public class SimpleEventParam_PlayerDoStep : SimpleEventParam
{
    public XoGameStep Step;
}

public class SimpleEventParam_GameOver : SimpleEventParam
{
    public GameState State;
}

public class SimpleEventParam_PlayerUnDoStep : SimpleEventParam
{
    public XoGameStep UnDoStep1;
    public XoGameStep UnDoStep2;
}