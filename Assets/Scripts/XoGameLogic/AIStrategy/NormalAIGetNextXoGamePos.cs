/// <summary>
/// 简单策略
/// </summary>
public class NormalAIGetNextXoGamePos : BaseAIGetNextXoGamePos
{
    public NormalAIGetNextXoGamePos(int selfPlayerNumber, int otherPlayerNumber) : base(selfPlayerNumber,
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

        return GetRandomPos(xoBoard);
    }
}