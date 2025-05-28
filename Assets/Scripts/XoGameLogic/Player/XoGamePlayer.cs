using System.Collections.Generic;

/// <summary>
/// 玩家基类，保存有玩家的信息
/// </summary>
public class XoGamePlayer
{
    //玩家的操作信息
    public List<int> history = new List<int>();
    //当前玩家代表的数字
    public int PlayerNumber { get; protected set; }
    //现在所在棋盘的引用
    protected XoGameBoard curGameBoard;

    public XoGamePlayer(XoGameBoard board, int playerNumber)
    {
        curGameBoard = board;
        PlayerNumber = playerNumber;
    }

    public void UnDoStep()
    {
        history.RemoveAt(history.Count - 1);
    }

    public bool DoStep()
    {
        var step = new XoGameStep() { PosIndex = GetNextPos(), Owner = this };
        history.Add(step.PosIndex);
        if (curGameBoard.SetBoardCellContent(step))
        {
            curGameBoard.simpleEventCenter.SendEvent(XOGameEventEnum.PlayerDoStep,
                new SimpleEventParam_PlayerDoStep() { Step = step });
            return true;
        }

        return false;
    }

    protected virtual int GetNextPos()
    {
        return curGameBoard.InputCache;
    }

    public virtual string GetPlayerName()
    {
        return "player" + PlayerNumber;
    }

    public virtual void Reset()
    {
        history.Clear();
    }

    public virtual void OnYouTurn()
    {
    }
}