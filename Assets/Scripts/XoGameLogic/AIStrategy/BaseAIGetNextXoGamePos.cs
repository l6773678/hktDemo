using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基础的AI逻辑类，可以拓展各种难度风格的AI，目前有三种 随机， 简单策略 ， 特定喜好的简单策略
/// </summary>
public class BaseAIGetNextXoGamePos
{
    protected int SelfPlayerNumber;
    protected int OtherPlayerNumber;

    public BaseAIGetNextXoGamePos(int selfPlayerNumber, int otherPlayerNumber)
    {
        SelfPlayerNumber = selfPlayerNumber;
        OtherPlayerNumber = otherPlayerNumber;
    }

    public virtual int GetNextStep(XoGameBoard board)
    {
        return GetRandomPos(board);
    }
    
    //通用函数，获取随机可下位置
    protected int GetRandomPos(XoGameBoard board)
    {
        List<int> emptyCells = new List<int>();
        for (int i = 0; i < XoGameBoard.BOARD_SIZE; i++)
        {
            if (board.GetBoardByIndex(i) == 0) emptyCells.Add(i);
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];
    }
    
    //通用函数，获取可能赢的位置
    protected bool CheckAndFindCanWinPos(XoGameBoard xoBoard, int playerNumber, out int posIndex)
    {
        var board = xoBoard.GetCurBoardCopy();
        posIndex = -1;
        for (int i = 0; i < XoGameBoard.BOARD_SIZE; i++)
        {
            if (board[i] != 0) continue;

            board[i] = playerNumber;
            xoBoard.CheckIsGameOver(board, out int winPlayerNumber);
            if (winPlayerNumber == playerNumber)
            {
                board[i] = 0;
                posIndex = i;
                return true;
            }

            board[i] = 0;
        }

        return false;
    }
}


