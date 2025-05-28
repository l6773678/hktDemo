using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 带有一定偏好的简单策略
/// </summary>
public class SpecialTragedyAIGetNextXoGamePos : BaseAIGetNextXoGamePos
{
    public SpecialTragedyAIGetNextXoGamePos(int selfPlayerNumber, int otherPlayerNumber) : base(selfPlayerNumber,
        otherPlayerNumber)
    {
    }

    public override int GetNextStep(XoGameBoard xoBoard)
    {
        // 先找自己能赢的下
        {
            if (CheckAndFindCanWinPos(xoBoard, SelfPlayerNumber, out int posIndex))
            {
                return posIndex;
            }
        }

        // 没找到就找对面能赢的地方防守
        {
            if (CheckAndFindCanWinPos(xoBoard, OtherPlayerNumber, out int posIndex))
            {
                return posIndex;
            }
        }

        var selfPlayer = xoBoard.GetPlayerByPlayerNumber(SelfPlayerNumber);
        if (selfPlayer == null)
        {
            return GetRandomPos(xoBoard);
        }

        //启用特殊策略，会按特定的规则下棋，大概就是先手的话会先下四个角
        var board = xoBoard.GetCurBoardCopy();

        //0 1 2
        //3 4 5
        //6 7 8
        if (selfPlayer.history.Count == 0) //不是第一手，切没下过，那就是后手
        {
            List<int> emptyCells = new List<int>() { 0, 2, 6, 8 };
            int outCome = emptyCells[Random.Range(0, emptyCells.Count)];
            return outCome;
        }

        if (selfPlayer.history.Count == 1)
        {
            if (selfPlayer.history[0] == 0)
            {
                if (board[1] == 0 && board[2] == 0)
                {
                    return 1;
                }

                if (board[3] == 0 && board[6] == 0)
                {
                    return 3;
                }
            }


            if (selfPlayer.history[0] == 2)
            {
                if (board[1] == 0 && board[0] == 0)
                {
                    return 1;
                }

                if (board[5] == 0 && board[8] == 0)
                {
                    return 5;
                }
            }

            if (selfPlayer.history[0] == 6)
            {
                if (board[3] == 0 && board[0] == 0)
                {
                    return 3;
                }

                if (board[7] == 0 && board[8] == 0)
                {
                    return 7;
                }
            }

            if (selfPlayer.history[0] == 8)
            {
                if (board[7] == 0 && board[6] == 0)
                {
                    return 7;
                }

                if (board[5] == 0 && board[2] == 0)
                {
                    return 5;
                }
            }
        }


        if (selfPlayer.history.Count == 2)
        {
            // 先找自己能赢的下
            {
                if (CheckAndFindCanWinPos(xoBoard, SelfPlayerNumber, out int posIndex))
                {
                    return posIndex;
                }
            }

            if (board[4] == 0)
            {
                return 4;
            }
        }

        return GetRandomPos(xoBoard);
    }
}