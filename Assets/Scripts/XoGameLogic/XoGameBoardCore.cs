using System.Collections.Generic;

public enum GameState
{
    PlayerWin,
    OverNoWinner,
    NoOver
}

public struct XoGameStep
{
    public int PosIndex;
    public XoGamePlayer Owner;
}
/// <summary>
/// 井字棋游戏的核心驱动逻辑类，一局游戏的承载者
/// </summary>
public class XoGameBoard
{
    public const int BOARD_SIZE = 9;

    private readonly int[,] winerChecker = new int[8, 3]
    {
        { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, //横 
        { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, //竖
        { 0, 4, 8 }, { 2, 4, 6 } //斜
    };
    //棋盘格子数据
    private List<int> board = new List<int>(BOARD_SIZE);
    //防止频繁newlist，简单优化处理，不能被外部持有
    private List<int> boardCopy = new List<int>(BOARD_SIZE);
    //全局操作纪录
    private Stack<XoGameStep> XoGameSteps = new Stack<XoGameStep>(BOARD_SIZE);
    //玩家对象
    private XoGamePlayer _player1;
    private XoGamePlayer _player2;
    //目前正在操作的玩家
    public XoGamePlayer PlayerCur{ get; private set; }
    //玩家手动输入缓存
    public int InputCache { get; private set; }
    //目前的游戏状态
    public GameState XoGameState { get; private set; } = GameState.NoOver;

    public readonly SimpleEventCenter simpleEventCenter = new SimpleEventCenter();
    public XoGameBoard()
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            board.Add(0);
            boardCopy.Add(0);
        }
    }
    //重置游戏
    public void Reset(XoGamePlayer player1, XoGamePlayer player2)
    {
        XoGameState = GameState.NoOver;
        _player1 = player1;
        _player2 = player2;
        XoGameSteps.Clear();
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            board[i] = 0;
        }

        if (XoGameHelper.instance.firstPlayerHand == FirstPlayerType.Player1)
        {
            PlayerCur = _player1;
        }
        else
        {
            PlayerCur = _player2;
        }

        PlayerCur?.OnYouTurn();
        simpleEventCenter.SendEvent(XOGameEventEnum.SwitchHand, new SimpleEventParam());
    }
    
    public XoGamePlayer GetPlayerByPlayerNumber(int playerNumber)
    {
        if (playerNumber == 1)
        {
            return _player1;
        }

        if (playerNumber == 2)
        {
            return _player2;
        }

        return null;
    }
    
    //获取当前输入
    public void InputAndDoStep(int posIndex)
    {
        InputCache = posIndex;
        DoPlayerStep();
    }
    
    //获取指定位置的棋盘内容
    public int GetBoardByIndex(int index)
    {
        if (index < board.Count)
        {
            return board[index];
        }

        return 0;
    }
    //获取现在棋盘内容的复制，主要是AI演算使用
    public List<int> GetCurBoardCopy()
    {
        for (var i = 0; i < board.Count; i++)
        {
            boardCopy[i] = board[i];
        }

        return boardCopy;
    }
    
    //交换输入玩家
    private void SwitchHand()
    {
        if (XoGameState != GameState.NoOver) return;
        if (PlayerCur == _player1)
        {
            PlayerCur = _player2;
        }
        else
        {
            PlayerCur = _player1;
        }

        simpleEventCenter.SendEvent(XOGameEventEnum.SwitchHand, new SimpleEventParam());
        PlayerCur?.OnYouTurn();
    }

    //当前玩家进行操作
    public void DoPlayerStep()
    {
        if (PlayerCur.DoStep())
        {
            CheckIsGameOverReal();
            SwitchHand();
        }
    }
    
    //实际落子
    public bool SetBoardCellContent(in XoGameStep step)
    {
        if (XoGameState != GameState.NoOver) return false;

        if (XoGameSteps.Count > 0 && XoGameSteps.Peek().Owner == step.Owner)
        {
            return false;
        }

        if (step.PosIndex < board.Count)
        {
            if (board[step.PosIndex] != 0)
            {
                return false;
            }

            board[step.PosIndex] = step.Owner.PlayerNumber;
            XoGameSteps.Push(step);
            return true;
        }

        return false;
    }
    
    //撤销上一步操作
    public void UnDoStep()
    {
        if (XoGameState != GameState.NoOver) return ;

        if (XoGameSteps.Count > 1)
        {
            var step1 = XoGameSteps.Pop();
            board[step1.PosIndex] = 0;
            step1.Owner.UnDoStep();

            var step2 = XoGameSteps.Pop();
            board[step2.PosIndex] = 0;
            step2.Owner.UnDoStep();

            simpleEventCenter.SendEvent(XOGameEventEnum.PlayerUnDoStep,
                new SimpleEventParam_PlayerUnDoStep() { UnDoStep1 = step1, UnDoStep2 = step2 });
        }
    }

    //检查对局状态，真实判断
    private void CheckIsGameOverReal()
    {
        XoGameState = CheckIsGameOver(board, out int winPlayerNumber);
        if (XoGameState != GameState.NoOver)
        {
            simpleEventCenter.SendEvent(XOGameEventEnum.GameOver, new SimpleEventParam_GameOver() { State = XoGameState });
        }
    }
    //检查对局状态，依赖外部的棋盘数据，不作真实判断
    public GameState CheckIsGameOver(List<int> curBoard, out int winPlayerNumber)
    {
        winPlayerNumber = -1;
        if (curBoard == null || curBoard.Count != BOARD_SIZE)
        {
            return GameState.OverNoWinner;
        }
        //0 1 2
        //3 4 5
        //6 7 8
        
        //先判断有没有人赢
        for (int i = 0; i < winerChecker.GetLength(0); i++)
        {
            int a = winerChecker[i, 0];
            int b = winerChecker[i, 1];
            int c = winerChecker[i, 2];

            if (curBoard[a] != 0 && curBoard[a] == curBoard[b] && curBoard[a] == curBoard[c])
            {
                if (curBoard[a] == 1)
                {
                    winPlayerNumber = 1;
                }

                if (curBoard[a] == 2)
                {
                    winPlayerNumber = 2;
                }

                return GameState.PlayerWin;
            }
        }
        
        //再判断是否平局
        foreach (var boardContent in curBoard)
        {
            if (boardContent == 0)
            {
                return GameState.NoOver;
            }
        }

        return GameState.OverNoWinner;
    }
}