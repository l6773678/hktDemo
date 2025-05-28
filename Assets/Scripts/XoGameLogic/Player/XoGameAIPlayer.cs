
/// <summary>
/// AI玩家基类，可通过设置策略达成不同的操作逻辑
/// </summary>
public class XoGameAIPlayer : XoGamePlayer
{
    private BaseAIGetNextXoGamePos aiHelper;

    public XoGameAIPlayer(BaseAIGetNextXoGamePos baseAI, XoGameBoard board, int playerNumber) : base(board, playerNumber)
    {
        PlayerNumber = 2;
        aiHelper = baseAI;
    }

    protected override int GetNextPos()
    {
        return aiHelper.GetNextStep(curGameBoard);
    }

    public override string GetPlayerName()
    {
        return "AiPlayer";
    }

    public override void OnYouTurn()
    {
        curGameBoard.DoPlayerStep();
    }
}

